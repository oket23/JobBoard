using JobBoard.Recruitment.Domain.Abstractions;
using JobBoard.Recruitment.Domain.Abstractions.Repositories;
using JobBoard.Recruitment.Domain.Abstractions.Services;
using JobBoard.Recruitment.Domain.Enums.Application;
using JobBoard.Recruitment.Domain.Models.Applications;
using JobBoard.Recruitment.Domain.Requests.Applications;
using JobBoard.Recruitment.Domain.Response;
using JobBoard.Recruitment.Domain.Response.Applications;
using JobBoard.Shared.Caching;
using JobBoard.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace JobBoard.Recruitment.Application.Services.Applications;

public class ApplicationsService : IApplicationsService
{
    private readonly IApplicationsRepository _applicationRepository;
    private readonly IJobsRepository _jobsRepository;
    private readonly ILogger<ApplicationsService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    private const string UserAppsPrefix = "apps:user";

    public ApplicationsService(
        IApplicationsRepository applicationRepository,
        IJobsRepository jobsRepository,
        ILogger<ApplicationsService> logger,
        IUnitOfWork unitOfWork,
        ICacheService cache)
    {
        _applicationRepository = applicationRepository;
        _jobsRepository = jobsRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task ChangeStatus(int id, ChangeApplicationStatusRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to change status for App ID: {AppId}. Target status: {Status}", id, request.Status);

        var app = await _applicationRepository.GetById(id, cancellationToken);
        if (app == null)
        {
            _logger.LogError("Application ID {AppId} not found for status update", id);
            throw new NotFoundException($"Application with id {id} not found");
        }

        var oldStatus = app.Status;
        app.Status = request.Status;
    
        _applicationRepository.Update(app);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Status updated: App ID {AppId} from {OldStatus} to {NewStatus}", id, oldStatus, request.Status);
    
        var userCachePrefix = $"{UserAppsPrefix}:{app.UserId}:";
        await _cache.RemoveByPrefixAsync(userCachePrefix);
        _logger.LogInformation("Invalidated user applications cache for User ID: {UserId}", app.UserId);
        
        //Тригерить подію для Notification Service
    }

    public async Task Create(int jobId, int userId, CreateApplicationRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing application: User {UserId} for Job {JobId}", userId, jobId);

        var job = await _jobsRepository.GetById(jobId, cancellationToken);
        if (job == null)
        {
            _logger.LogWarning("Application failed: Job ID {JobId} does not exist", jobId);
            throw new NotFoundException($"Job post with id {jobId} not found");
        }

        if (!job.IsActive)
        {
            _logger.LogWarning("Application rejected: Job ID {JobId} is inactive", jobId);
            throw new BadRequestException("You cannot apply for a closed job position.");
        }

        var app = new UserApplication
        {
            JobPostId = jobId,
            UserId = userId,
            CoverLetter = request.CoverLetter,
            Status = ApplicationStatusEnum.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _applicationRepository.Add(app);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Application ID {AppId} created for User {UserId}", app.Id, userId);
    
        await _cache.RemoveByPrefixAsync($"{UserAppsPrefix}:{userId}:");
        _logger.LogDebug("Cleared application cache for User {UserId}", userId);
        
        //Тригерить подію для Notification Service
    }

    public async Task<ResponseList<UserApplicationResponse>> GetUserApplications(int userId, UserApplicationRequest request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeyGenerator.GetCacheKey($"{UserAppsPrefix}:{userId}", request);

        _logger.LogDebug("Checking cache for user applications. UserID: {UserId}, CacheKey: {CacheKey}", userId, cacheKey);

        var cached = await _cache.GetAsync<ResponseList<UserApplicationResponse>>(cacheKey, cancellationToken);
        if (cached != null)
        {
            _logger.LogInformation("Cache hit: Returning applications for UserID: {UserId}", userId);
            return cached;
        }

        _logger.LogInformation("Cache miss: Fetching applications from DB for UserID: {UserId}. Offset: {Offset}, Limit: {Limit}", userId, request.Offset, request.Limit);

        var apps = await _applicationRepository.GetAllForUser(userId, request, cancellationToken);
        var response = apps.ToResponseList(UserAppToResponse);
        
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromSeconds(60), cancellationToken);
    
        _logger.LogInformation("Successfully fetched {Count} applications for UserID: {UserId} and updated cache", response.Items.Count(), userId);
    
        return response;
    }

    private UserApplicationResponse UserAppToResponse(UserApplication item) => new()
    {
        Id = item.Id,
        JobPostId = item.JobPostId,
        JobTitle = item.JobPost?.Title,
        CoverLetter = item.CoverLetter,
        Status = item.Status,
        CreatedAt = item.CreatedAt
    };
}
