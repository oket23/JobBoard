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

    public async Task<ResponseList<UsersBatchResponse>> GetUsersBatch(GetUsersBatchRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching users batch. Count of IDs: {IdCount}, Offset: {Offset}, Limit: {Limit}", 
            request.Ids?.Count() ?? 0, request.Offset, request.Limit);
        
        if (request.Ids == null || !request.Ids.Any())
        {
            _logger.LogDebug("Batch request IDs list is empty.");
            return new ResponseList<UsersBatchResponse>
            {
                Items = new List<UsersBatchResponse>(),
                TotalCount = 0,
                Limit = request.Limit,
                Offset = request.Offset,
                Page = 1
            };
        }
        
        var pagedUsers = await _userRepository.GetUsersByIds(request, cancellationToken);
        
        return new ResponseList<UsersBatchResponse>
        {
            Items = pagedUsers.Items.Select(u => new UsersBatchResponse
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender
            }).ToList(),
            TotalCount = pagedUsers.TotalCount,
            Limit = pagedUsers.Limit,
            Offset = pagedUsers.Offset,
            Page = pagedUsers.Page
        };
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
        
        user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;
        user.Gender = request.Gender ?? user.Gender;
        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;
        //user.Email = request.Email ?? user.Email;
        user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;
    
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