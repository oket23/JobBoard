namespace JobBoard.Recruitment.Domain.Response.Jobs;

public class JobResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Requirements { get; set; }
    public required bool IsActive { get; set; }
}