using JobBoard.Identity.Domain.Abstractions.Repositories;
using JobBoard.Identity.Domain.Models.RefreshTokens;
using JobBoard.Identity.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Identity.Infrastructure.Repositories;

public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(JobBoardIdentityContext context) : base(context)
    {
    }

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return Set<RefreshToken>().Include(x => x.User) 
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }
}