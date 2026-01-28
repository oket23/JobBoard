using JobBoard.Recruitment.Domain.Enums.Application;
using JobBoard.Recruitment.Domain.Models.Base;
using JobBoard.Recruitment.Domain.Models.JobPosts;

namespace JobBoard.Recruitment.Domain.Models.Applications;

public class UserApplication : BaseEntity
{
    public int Id { get; set; }
    public int JobPostId { get; set; }
    public JobPost? JobPost { get; set; }
    public int UserId { get; set; }
    public required string CoverLetter { get; set; }
    public ApplicationStatusEnum Status { get; set; } = ApplicationStatusEnum.Pending;
}

