using System.Runtime.Serialization;

namespace JobBoard.Identity.Domain.Enums.Users;

public enum UserRole
{
    [EnumMember(Value = "user")]
    User = 1,
    [EnumMember(Value = "admin")]
    Admin = 2
}