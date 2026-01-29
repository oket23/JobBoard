using JobBoard.Identity.Domain.Requests.Users;
using JobBoard.Identity.Domain.Response;
using JobBoard.Identity.Domain.Response.Users;

namespace JobBoard.Identity.Domain.Abstractions.Services;

public interface IUserService
{
    Task<ResponseList<UserResponse>> GetAll(UserRequest request, CancellationToken cancellationToken);
    Task<UserResponse> GetById(int id, CancellationToken cancellationToken);
    Task<ResponseList<UsersBatchResponse>> GetUsersBatch(GetUsersBatchRequest request, CancellationToken cancellationToken);
    Task Update(int id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task Delete(int id, CancellationToken cancellationToken);
}
