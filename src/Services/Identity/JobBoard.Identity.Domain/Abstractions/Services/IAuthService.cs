using JobBoard.Identity.Domain.Requests.Auth;
using JobBoard.Identity.Domain.Response.Auth;
using JobBoard.Identity.Domain.Response.Users;

namespace JobBoard.Identity.Domain.Abstractions.Services;

public interface IAuthService
{
    Task<AuthResponse> Register(RegisterUserRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> Login(LoginUserRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshToken(string accessToken, string refreshToken, CancellationToken cancellationToken = default);
    Task<UserResponse> GetUserProfile(int userId, CancellationToken cancellationToken = default);
}