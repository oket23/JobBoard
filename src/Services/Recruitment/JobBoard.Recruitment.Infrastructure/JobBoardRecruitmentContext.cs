using JobBoard.Recruitment.Domain.Abstractions;
using JobBoard.Recruitment.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Recruitment.Infrastructure;

public class JobBoardRecruitmentContext : DbContext, IUnitOfWork
{
    public JobBoardRecruitmentContext(DbContextOptions<JobBoardRecruitmentContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobPostConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}