using JobBoard.Identity.Domain.Models.Users;
using JobBoard.Identity.Domain.Requests.Users;
using JobBoard.Identity.Domain.Response;

namespace JobBoard.Identity.Domain.Abstractions.Repositories;

public interface IUserRepository
{
    void Add(User user);
    void Update(User user);
    void Delete(User user);
    Task<ResponseList<User>> GetAll(UserRequest request, CancellationToken cancellationToken);
    ValueTask<User?> GetById(int id, CancellationToken cancellationToken);
    Task<User?> GetByEmail(string email, CancellationToken cancellationToken);
    Task<bool> IsEmailExist(string email, CancellationToken cancellationToken);
}