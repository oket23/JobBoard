using JobBoard.Identity.Domain.Abstractions;
using JobBoard.Identity.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Identity.Infrastructure;

public class JobBoardIdentityContext : DbContext, IUnitOfWork
{
    public JobBoardIdentityContext(DbContextOptions<JobBoardIdentityContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}