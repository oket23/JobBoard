using JobBoard.Identity.Domain.Enums.Users;

namespace JobBoard.Identity.Domain.Requests.Auth;

public class RegisterUserRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; } 
    public required string FirstName { get; set; } 
    public required string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }  
    public UserGender Gender { get; set; } 
}


