using JobBoard.Recruitment.Domain.Enums.Application;

namespace JobBoard.Recruitment.Domain.Requests.Applications;

public class ChangeApplicationStatusRequest
{
    public ApplicationStatusEnum Status { get; set; }
}