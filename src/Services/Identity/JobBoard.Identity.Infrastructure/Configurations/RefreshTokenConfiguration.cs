using JobBoard.Identity.Domain.Models.RefreshTokens;
using JobBoard.Identity.Domain.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Identity.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Token)
            .IsRequired();
        
        builder.HasIndex(x => x.Token)
            .IsUnique();
        
        builder.Property(x => x.JwtId)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);
        
        builder.HasQueryFilter(x => x.DeletedAt == null);
    }
}