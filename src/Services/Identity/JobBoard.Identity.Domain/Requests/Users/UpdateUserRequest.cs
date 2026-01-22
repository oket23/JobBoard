using JobBoard.Identity.Domain.Enums.Users;

namespace JobBoard.Identity.Domain.Requests.Users;

public class UpdateUserRequest
{
    public required string Email { get; set; } //later change to method with AUTH2.0
    public required string FirstName { get; set; } 
    public required string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }  
    public UserRole Role { get; set; } 
    public UserGender Gender { get; set; } 
}