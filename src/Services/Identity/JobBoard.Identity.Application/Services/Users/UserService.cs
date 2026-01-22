using JobBoard.Identity.Application.Mappers;
using JobBoard.Identity.Domain.Abstractions;
using JobBoard.Identity.Domain.Abstractions.Repositories;
using JobBoard.Identity.Domain.Abstractions.Services;
using JobBoard.Identity.Domain.Models.Users;
using JobBoard.Identity.Domain.Requests.Users;
using JobBoard.Identity.Domain.Response;
using JobBoard.Identity.Domain.Response.Users;
using Microsoft.Extensions.Logging;

namespace JobBoard.Identity.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<ResponseList<User>> GetAll(UserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching users with filter. Offset: {Offset}, Limit: {Limit}", request.Offset, request.Limit);
        
        return await _userRepository.GetAll(request, cancellationToken);
    }

    public async Task<UserResponse> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching user by id: {Id}", id);
        
        var user = await _userRepository.GetById(id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User with id {Id} not found", id);
            throw new KeyNotFoundException($"User with id {id} not found"); 
        }

        return user.ToResponse();
    }

    public async Task Update(int id, UpdateUserRequest request, CancellationToken cancellationToken) 
    {
        _logger.LogInformation("Updating user with id: {Id}", id);
        
        var user = await _userRepository.GetById(id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User with id {Id} not found for update", id);
            throw new KeyNotFoundException($"User with id {id} not found");
        };
        
        user.DateOfBirth = request.DateOfBirth;
        user.Gender = request.Gender;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.DateOfBirth = request.DateOfBirth;
    
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("User with id: {Id} updated successfully", id);
    }

    public async Task Delete(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting user with id: {Id}", id);
        
        var user = await _userRepository.GetById(id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User with id {Id} not found for deletion", id);
            throw new KeyNotFoundException($"User with id {id} not found");
        }

        _userRepository.Delete(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("User with id: {Id} deleted successfully", id);
    }
}