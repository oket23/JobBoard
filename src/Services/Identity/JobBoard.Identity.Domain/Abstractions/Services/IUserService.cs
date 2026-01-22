using JobBoard.Identity.Domain.Abstractions.Repositories;
using JobBoard.Identity.Domain.Models.Users;
using JobBoard.Identity.Domain.Requests.Users;
using JobBoard.Identity.Domain.Response;
using JobBoard.Identity.Domain.Response.Users;

namespace JobBoard.Identity.Domain.Abstractions.Services;

public interface IUserService
{
    Task<ResponseList<User>> GetAll(UserRequest request, CancellationToken cancellationToken);
    Task<UserResponse> GetById(int id, CancellationToken cancellationToken);
    Task Update(int id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task Delete(int id, CancellationToken cancellationToken);
}
