using JobBoard.Identity.Domain.Enums.Users;

namespace JobBoard.Identity.Domain.Abstractions;

public interface IJwtGenerator
{
    string GenerateAccessToken(int userId, string email, UserRole role);
    string GenerateRefreshToken();
}