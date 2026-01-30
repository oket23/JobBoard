using JobBoard.Identity.Domain.Enums.Users;

namespace JobBoard.Identity.Domain.Requests.Users;

public class UpdateUserRequest
{
    //public required string Email { get; set; } //later change to method with AUTH2.0
    public string? FirstName { get; set; } 
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }  
    public UserGender? Gender { get; set; } 
}