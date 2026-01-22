using JobBoard.Identity.Domain.Response.Users;

namespace JobBoard.Identity.Domain.Response.Auth;

public class AuthResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime AccessTokenExpiry { get; set; }
    public required UserResponse User { get; set; }
}