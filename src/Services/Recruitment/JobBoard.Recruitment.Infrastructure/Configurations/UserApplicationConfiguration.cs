using JobBoard.Recruitment.Domain.Models.Applications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Recruitment.Infrastructure.Configurations;

public class UserApplicationConfiguration : IEntityTypeConfiguration<UserApplication>
{
    public void Configure(EntityTypeBuilder<UserApplication> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.CoverLetter)
            .IsRequired()
            .HasMaxLength(2000);
        
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.HasIndex(x => x.UserId);
        
        builder.HasQueryFilter(x => x.DeletedAt == null);
    }
}