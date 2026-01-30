using JobBoard.Recruitment.Domain.Abstractions.Repositories;
using JobBoard.Recruitment.Domain.Enums.Application;
using JobBoard.Recruitment.Domain.Models.Applications;
using JobBoard.Recruitment.Domain.Requests.Applications;
using JobBoard.Recruitment.Domain.Response;
using JobBoard.Recruitment.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Recruitment.Infrastructure.Repositories;

public class ApplicationsRepository : RepositoryBase<UserApplication>, IApplicationsRepository
{
    public ApplicationsRepository(JobBoardRecruitmentContext context) : base(context)
    {
    }
    
    public async Task<ResponseList<UserApplication>> GetAllForUser(int userId, UserApplicationRequest request, CancellationToken cancellationToken)
    {
        var query = Set<UserApplication>()
            .AsNoTracking()
            .Include(a => a.JobPost)
            .Where(x => x.UserId == userId)
            .AsQueryable();
        
        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }
        
        query = query.OrderByDescending(x => x.CreatedAt);
        
        var count = await query.CountAsync(cancellationToken);

       
        var items = await query
            .Skip(request.Offset) 
            .Take(request.Limit) 
            .ToListAsync(cancellationToken);
        
        return new ResponseList<UserApplication>
        {
            Items = items,
            TotalCount = count,
            Limit = request.Limit,
            Offset = request.Offset
        };
    }
    public override async ValueTask<UserApplication?> GetById(int id, CancellationToken cancellationToken)
    {
        return await Set<UserApplication>()
            .Include(x => x.JobPost)
            .SingleOrDefaultAsync(x => x.Id == id,cancellationToken);
    }
}