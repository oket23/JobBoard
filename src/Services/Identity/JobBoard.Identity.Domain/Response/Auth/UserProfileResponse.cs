using JobBoard.Identity.Domain.Enums.Users;

namespace JobBoard.Identity.Domain.Response.Auth;

public class UserProfileResponse
{
    public required string Email { get; set; }
    public required string FirstName { get; set; } 
    public required string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }  
    public UserRole Role { get; set; } 
    public UserGender Gender { get; set; } 
}