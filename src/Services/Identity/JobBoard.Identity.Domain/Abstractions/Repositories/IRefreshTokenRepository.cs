using JobBoard.Identity.Domain.Models.RefreshTokens;

namespace JobBoard.Identity.Domain.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);
    
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
}