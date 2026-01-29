using JobBoard.Recruitment.Domain.Enums.Application;

namespace JobBoard.Recruitment.Domain.Requests.Applications;

public class UserApplicationRequest : PaginationBase
{
    public ApplicationStatusEnum? Status { get; set; }
}