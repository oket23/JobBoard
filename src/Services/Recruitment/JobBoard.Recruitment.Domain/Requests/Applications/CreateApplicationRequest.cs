namespace JobBoard.Recruitment.Domain.Requests.Applications;

public class CreateApplicationRequest
{
    public required string CoverLetter { get; set; }
    public required string FirstName { get; set; }
    public required string Email { get; set; }
    public required int UserId { get; set; }
    public required int JobId { get; set; }
}