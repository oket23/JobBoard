using JobBoard.Recruitment.Domain.Enums.Application;
using JobBoard.Recruitment.Domain.Models.Applications;
using JobBoard.Recruitment.Domain.Requests.Applications;
using JobBoard.Recruitment.Domain.Response;

namespace JobBoard.Recruitment.Domain.Abstractions.Repositories;

public interface IApplicationsRepository
{
    void Add(UserApplication job);
    void Update(UserApplication job);
    void Delete(UserApplication job);
    Task<ResponseList<UserApplication>> GetAllForUser(int userId, UserApplicationRequest request, CancellationToken cancellationToken);
    ValueTask<UserApplication?> GetById(int id, CancellationToken cancellationToken);
}