using JobBoard.Identity.Domain.Enums.Users;

namespace JobBoard.Identity.Domain.Response.Users;

public class UsersBatchResponse
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; } 
    public required string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }  
    public UserGender Gender { get; set; }
}