using JobBoard.Recruitment.Domain.Abstractions.Repositories;
using JobBoard.Recruitment.Domain.Models.JobPosts;
using JobBoard.Recruitment.Domain.Requests.Jobs;
using JobBoard.Recruitment.Domain.Response;
using JobBoard.Recruitment.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Recruitment.Infrastructure.Repositories;

public class JobsRepository : RepositoryBase<JobPost>, IJobsRepository
{
    public JobsRepository(JobBoardRecruitmentContext context) : base(context)
    {
    }

    public async Task<ResponseList<JobPost>> GetAll(JobRequest request, CancellationToken cancellationToken)
    {
        var query = Set<JobPost>().AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            query = query.Where(j => j.Title.Contains(request.Title));
        }
        
        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            query = query.Where(j => j.Description.Contains(request.Description));
        }
        
        if (!string.IsNullOrWhiteSpace(request.Requirements))
        {
            query = query.Where(j => j.Requirements.Contains(request.Requirements));
        }
        
        if (request.IsActive.HasValue)
        {
            query = query.Where(j => j.IsActive == request.IsActive.Value);
        }
        
        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip(request.Offset)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return new ResponseList<JobPost>
        {
            Items = items,
            TotalCount = count,
            Limit = request.Limit,
            Offset = request.Offset,
            Page = request.Offset / request.Limit,
        };
    }
}