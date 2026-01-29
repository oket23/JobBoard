using JobBoard.Recruitment.Domain.Requests.Jobs;
using JobBoard.Recruitment.Domain.Response;
using JobBoard.Recruitment.Domain.Response.Jobs;

namespace JobBoard.Recruitment.Domain.Abstractions.Services;

public interface IJobsServices
{
    Task<ResponseList<JobResponse>> GetAll(JobRequest request, CancellationToken cancellationToken);
    Task Create(CreateJobRequest request, CancellationToken cancellationToken);
    Task Delete(int id, CancellationToken cancellationToken);
}
