using System.Runtime.Serialization;

namespace JobBoard.Identity.Domain.Enums.Users;

public enum UserGender
{
    [EnumMember(Value = "male")]
    Male = 1,
    [EnumMember(Value = "female")]
    Female = 2,
    [EnumMember(Value = "not_specified")]
    NotSpecified = 3
}