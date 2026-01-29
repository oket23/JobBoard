using JobBoard.Recruitment.Domain.Requests.Applications;
using JobBoard.Recruitment.Domain.Response;
using JobBoard.Recruitment.Domain.Response.Applications;

namespace JobBoard.Recruitment.Domain.Abstractions.Services;

public interface IApplicationsService
{
    Task ChangeStatus(int id, ChangeApplicationStatusRequest request, CancellationToken cancellationToken);
    Task<ResponseList<UserApplicationResponse>> GetUserApplications(int userId, UserApplicationRequest request, CancellationToken cancellationToken);
    Task Create(int jobId, int userId, CreateApplicationRequest request, CancellationToken cancellationToken);
}