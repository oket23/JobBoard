using JobBoard.Recruitment.Domain.Models.JobPosts;
using JobBoard.Recruitment.Domain.Requests.Jobs;
using JobBoard.Recruitment.Domain.Response;

namespace JobBoard.Recruitment.Domain.Abstractions.Repositories;

public interface IJobsRepository
{
    void Add(JobPost job);
    void Update(JobPost job);
    void Delete(JobPost job);
    Task<ResponseList<JobPost>> GetAll(JobRequest request, CancellationToken cancellationToken);
    ValueTask<JobPost?> GetById(int id, CancellationToken cancellationToken);
}