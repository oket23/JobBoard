using JobBoard.Identity.Domain.Enums.Users;
using JobBoard.Identity.Domain.Models.Base;
using JobBoard.Identity.Domain.Models.RefreshTokens;

namespace JobBoard.Identity.Domain.Models.Users;

public class User : BaseEntity
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; } 
    public required string FirstName { get; set; } 
    public required string LastName { get; set; } 
    public UserRole Role { get; set; } 
    public UserGender Gender { get; set; } 
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}