using System.Runtime.Serialization;

namespace JobBoard.Recruitment.Domain.Enums.Application;

public enum ApplicationStatusEnum
{
    [EnumMember(Value = "pending")]
    Pending = 0,
    [EnumMember(Value = "reviewed")]
    Reviewed = 1,
    [EnumMember(Value = "rejected")]
    Rejected = 2,
    [EnumMember(Value = "approved")]
    Approved = 3
}