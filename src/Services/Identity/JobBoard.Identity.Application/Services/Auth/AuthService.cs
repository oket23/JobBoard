using System.IdentityModel.Tokens.Jwt;
using JobBoard.Identity.Application.Mappers;
using JobBoard.Identity.Domain.Abstractions;
using JobBoard.Identity.Domain.Abstractions.Repositories;
using JobBoard.Identity.Domain.Abstractions.Services;
using JobBoard.Identity.Domain.DTOs;
using JobBoard.Identity.Domain.Enums.Users;
using JobBoard.Identity.Domain.Models.RefreshTokens;
using JobBoard.Identity.Domain.Models.Users;
using JobBoard.Identity.Domain.Requests.Auth;
using JobBoard.Identity.Domain.Response.Auth;
using JobBoard.Identity.Domain.Response.Users;
using JobBoard.Shared.Exceptions;
using JobBoard.Shared.Hash;
using Microsoft.Extensions.Options;

namespace JobBoard.Identity.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IJwtGenerator _generator;
    private readonly JwtSettings _jwtSettings;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IJwtGenerator generator, IUserRepository userRepository, IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository, IOptions<JwtSettings> jwtOptions)
    {
        _generator = generator;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponse> Login(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmail(request.Email, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException($"User with email {request.Email} not found");
        }

        if (!HasherUtil.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new BadRequestException("Invalid password");
        }

        return await UserToAuthResponse(user, cancellationToken);
    }
    
    public async Task<AuthResponse> Register(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByEmail(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new ConflictException($"User with email {request.Email} already exists");
        }
        
        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = HasherUtil.HashPassword(request.Password),
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };
        
        _userRepository.Add(user);
        
        return await UserToAuthResponse(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshToken(string accessToken, string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);
        
        if (storedToken == null)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }

        if (storedToken.Invalidated)
        {
            throw new UnauthorizedException("Token is invalidated");
        }

        if (storedToken.ExpiryDate < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Token expired");
        }
        
        if (storedToken.Used)
        {
            storedToken.Invalidated = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException("Token already used. Security alert!");
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);
            var jti = jwtToken.Id;

            if (storedToken.JwtId != jti)
            {
                throw new UnauthorizedException("Token mismatch");
            }
        }
        catch (Exception)
        {
            throw new UnauthorizedException("Invalid access token format");
        }

        storedToken.Used = true;
        
        var user = await _userRepository.GetById(storedToken.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return await UserToAuthResponse(user, cancellationToken);
    }

    public async Task<UserResponse> GetUserProfile(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetById(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"User with id {userId} not found");
        }
        return user.ToResponse();
    }
    
    private async Task<AuthResponse> UserToAuthResponse(User user, CancellationToken cancellationToken)
    {
        var accessToken = _generator.GenerateAccessToken(user.Id, user.Email, user.Role);
        var refreshTokenString = _generator.GenerateRefreshToken();
        
        var handler = new JwtSecurityTokenHandler();
        var tokenObj = handler.ReadJwtToken(accessToken);
        var jti = tokenObj.Id;
        
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshTokenString,
            JwtId = jti,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshExpiryDays),
            Used = false,
            Invalidated = false
        };

        _refreshTokenRepository.Add(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString,
            User = user.ToResponse()
        };
    }
}