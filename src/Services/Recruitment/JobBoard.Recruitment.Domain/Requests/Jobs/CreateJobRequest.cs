namespace JobBoard.Recruitment.Domain.Requests.Jobs;

public class CreateJobRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Requirements { get; set; }
}