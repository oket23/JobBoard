namespace JobBoard.Recruitment.Domain.Requests.Jobs;

public class JobRequest : PaginationBase
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public bool? IsActive { get; set; }
}