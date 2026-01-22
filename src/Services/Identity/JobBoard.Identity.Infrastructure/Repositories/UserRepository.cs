using JobBoard.Identity.Domain.Abstractions.Repositories;
using JobBoard.Identity.Domain.Models.Users;
using JobBoard.Identity.Domain.Requests.Users;
using JobBoard.Identity.Domain.Response;
using JobBoard.Identity.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Identity.Infrastructure.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(JobBoardIdentityContext context) : base(context)
    {
    }

    public async Task<ResponseList<User>> GetAll(UserRequest request, CancellationToken cancellationToken)
    {
        var query = Set<User>().AsQueryable();
        
        if (request.Id.HasValue)
        {
            query = query.Where(u => u.Id == request.Id.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            query = query.Where(u => u.Email == request.Email);
        }

        if (request.Role.HasValue)
        {
            query = query.Where(u => u.Role == request.Role.Value);
        }

        if (request.Gender.HasValue)
        {
            query = query.Where(u => u.Gender == request.Gender.Value);
        }

        if (request.BornFrom.HasValue)
        {
            query = query.Where(u => u.DateOfBirth >= request.BornFrom.Value);
        }

        if (request.BornTo.HasValue)
        {
            query = query.Where(u => u.DateOfBirth <= request.BornTo.Value);
        }

        if (request.CreatedFrom.HasValue)
        {
            query = query.Where(u => u.CreatedAt >= request.CreatedFrom.Value);
        }

        if (request.CreatedTo.HasValue)
        {
            query = query.Where(u => u.CreatedAt <= request.CreatedTo.Value);
        }
        
        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip(request.Offset)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        return new ResponseList<User>
        {
            Items = items,
            TotalCount = count,
            Limit = request.Limit,
            Offset = request.Offset,
            Page = request.Offset / request.Limit,
        };
    }


    public Task<User?> GetByEmail(string email, CancellationToken cancellationToken)
    {
        return Set<User>().FirstOrDefaultAsync(u => u.Email.Equals(email), cancellationToken);
    }

    public Task<bool> IsEmailExist(string email, CancellationToken cancellationToken)
    {
        return Set<User>().AnyAsync(u => u.Email.Equals(email.Trim()), cancellationToken);
    }
}