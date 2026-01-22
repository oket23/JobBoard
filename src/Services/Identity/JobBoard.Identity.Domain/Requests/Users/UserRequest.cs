using JobBoard.Identity.Domain.Enums.Users;

namespace JobBoard.Identity.Domain.Requests.Users;

public class UserRequest : PaginationBase
{
    public int? Id { get; set; }
    public string? Email { get; set; }
    public UserRole? Role { get; set; }
    public UserGender? Gender { get; set; }
    public DateTime? BornFrom { get; set; }
    public DateTime? BornTo { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}