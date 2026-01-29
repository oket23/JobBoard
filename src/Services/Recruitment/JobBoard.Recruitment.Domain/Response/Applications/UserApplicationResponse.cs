using JobBoard.Recruitment.Domain.Enums.Application;

namespace JobBoard.Recruitment.Domain.Response.Applications;

public class UserApplicationResponse
{
    public int Id { get; set; }
    public int JobPostId { get; set; }
    public string? JobTitle { get; set; }
    public required string CoverLetter { get; set; }
    public DateTime CreatedAt { get; set; }
    public ApplicationStatusEnum Status { get; set; }
}
