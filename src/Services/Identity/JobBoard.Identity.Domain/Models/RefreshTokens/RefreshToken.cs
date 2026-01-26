using JobBoard.Identity.Domain.Models.Base;
using JobBoard.Identity.Domain.Models.Users;

namespace JobBoard.Identity.Domain.Models.RefreshTokens;

public class RefreshToken : BaseEntity
{
    public int Id { get; set; }
    public required string Token { get; set; }
    public required string JwtId { get; set; } 
    
    public DateTime ExpiryDate { get; set; } 
    
    public bool Used { get; set; } = false; 
    public bool Invalidated { get; set; } = false; 
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}