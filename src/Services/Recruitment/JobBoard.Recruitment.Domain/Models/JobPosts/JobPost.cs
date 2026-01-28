using JobBoard.Recruitment.Domain.Models.Applications;
using JobBoard.Recruitment.Domain.Models.Base;

namespace JobBoard.Recruitment.Domain.Models.JobPosts;

public class JobPost : BaseEntity
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Requirements { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<UserApplication> Applications { get; set; } = new List<UserApplication>();
}