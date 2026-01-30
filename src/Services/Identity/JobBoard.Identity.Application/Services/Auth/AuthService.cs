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
using JobBoard.Shared.Events.User;
using JobBoard.Shared.Exceptions;
using JobBoard.Shared.Hash;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JobBoard.Identity.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IJwtGenerator _generator;
    private readonly ILogger<AuthService> _logger;
    private readonly JwtSettings _jwtSettings;
    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IJwtGenerator generator, IUserRepository userRepository, IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepository, IOptions<JwtSettings> jwtOptions, IPublishEndpoint publishEndpoint, ILogger<AuthService> logger)
    {
        _generator = generator;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = refreshTokenRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
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
        
        _logger.LogInformation("User {UserId} logged in successfully", user.Id);
        
        await _publishEndpoint.Publish(new UserLoginEvent(user.Id, user.Email, user.FirstName, DateTime.UtcNow), cancellationToken);
        
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
            Gender = request.Gender,
            Role = UserRole.User,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };
        
        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        await _publishEndpoint.Publish(new UserRegisteredEvent(user.Id, user.Email, user.FirstName), cancellationToken);
        
        return await UserToAuthResponse(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        
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
            var jwtToken = handler.ReadJwtToken(request.AccessToken);
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

    public async Task<UserProfileResponse> GetUserProfile(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetById(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"User with id {userId} not found");
        }

        return new UserProfileResponse
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Role = user.Role,
            Gender = user.Gender
        };
    }
    
    private async Task<AuthResponse> UserToAuthResponse(User user, CancellationToken cancellationToken)
    {
        var accessToken = _generator.GenerateAccessToken(user.Id, user.Email,user.FirstName, user.Role);
        var refreshTokenString = _generator.GenerateRefreshToken();
        
        var handler = new JwtSecurityTokenHandler();
        var tokenObj = handler.ReadJwtToken(accessToken);
        var jti = tokenObj.Id;
        var expiry = tokenObj.ValidTo;
        
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
            AccessTokenExpiry = expiry,
            User = user.ToResponse()
        };
    }
}