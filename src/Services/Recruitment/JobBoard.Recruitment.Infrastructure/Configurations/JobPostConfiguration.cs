using JobBoard.Recruitment.Domain.Models.JobPosts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Recruitment.Infrastructure.Configurations;

public class JobPostConfiguration : IEntityTypeConfiguration<JobPost>
{
    public void Configure(EntityTypeBuilder<JobPost> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.Requirements)
            .IsRequired()
            .HasMaxLength(4000);
        
        builder.HasMany(jp => jp.Applications)
            .WithOne(a => a.JobPost)
            .HasForeignKey(a => a.JobPostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasQueryFilter(x => x.DeletedAt == null);
    }
}