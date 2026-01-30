using JobBoard.Recruitment.Domain.Abstractions;
using JobBoard.Recruitment.Domain.Abstractions.Repositories;
using JobBoard.Recruitment.Domain.Abstractions.Services;
using JobBoard.Recruitment.Domain.Models.Applications;
using JobBoard.Recruitment.Domain.Models.JobPosts;
using JobBoard.Recruitment.Domain.Requests.Jobs;
using JobBoard.Recruitment.Domain.Response;
using JobBoard.Recruitment.Domain.Response.Jobs;
using JobBoard.Shared.Caching;
using JobBoard.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace JobBoard.Recruitment.Application.Services.Jobs;

public class JobsService : IJobsService
{
    private readonly IJobsRepository _repository;
    private readonly ILogger<JobsService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    private const string JobsPrefix = "jobs:list";

    public JobsService(IJobsRepository repository, ILogger<JobsService> logger, IUnitOfWork unitOfWork, ICacheService cache)
    {
        _repository = repository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<ResponseList<JobResponse>> GetAll(JobRequest request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeyGenerator.GetCacheKey(JobsPrefix, request);
    
        var cached = await _cache.GetAsync<ResponseList<JobResponse>>(cacheKey, cancellationToken);
        if (cached != null)
        {
            _logger.LogInformation("Cache hit for jobs list. Key: {CacheKey}", cacheKey);
            return cached;
        }

        _logger.LogInformation("Cache miss. Fetching jobs from DB. Filter: Title={Title}, Offset={Offset}, Limit={Limit}", request.Title ?? "None", request.Offset, request.Limit);

        var jobs = await _repository.GetAll(request, cancellationToken);
        var response = jobs.ToResponseList(ToJobResponse);

        await _cache.SetAsync(cacheKey, response, TimeSpan.FromSeconds(60), cancellationToken); 
        _logger.LogDebug("Jobs list cached with key: {CacheKey}", cacheKey);

        return response;
    }

    public async Task Create(CreateJobRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting creation of job post: {Title}", request.Title);

        var job = new JobPost
        {
            Title = request.Title,
            Description = request.Description,
            Requirements = request.Requirements,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Applications = new List<UserApplication>()
        };

        _repository.Add(job);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully persisted Job ID: {JobId} to database", job.Id);
    
        await _cache.RemoveByPrefixAsync(JobsPrefix);
        _logger.LogInformation("Invalidated cache for prefix: {Prefix}", JobsPrefix);
    }

    public async Task Delete(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request to delete job ID: {JobId}", id);

        var job = await _repository.GetById(id, cancellationToken);
        if (job == null) 
        {
            _logger.LogWarning("Delete failed: Job ID {JobId} not found", id);
            throw new NotFoundException($"Job with id {id} not found");
        }

        _repository.Delete(job);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Job ID {JobId} permanently removed from database", id);
    
        await _cache.RemoveByPrefixAsync(JobsPrefix);
        _logger.LogDebug("Cache cleared for prefix: {Prefix} after deletion", JobsPrefix);
    }
    
    public async Task Update(int id,UpdateJobRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request to update job ID: {JobId}", id);

        var job = await _repository.GetById(id, cancellationToken);
        if (job == null) 
        {
            _logger.LogWarning("Update failed: Job ID {JobId} not found", id);
            throw new NotFoundException($"Job with id {id} not found");
        }
        
        job.Title = request.Title ??  job.Title;
        job.Description = request.Description ??  job.Description;
        job.Requirements = request.Requirements ??  job.Requirements;
        job.IsActive = request.IsActive ??  job.IsActive;
        
        _repository.Update(job);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Job ID {JobId} Updated", id);
    
        await _cache.RemoveByPrefixAsync(JobsPrefix);
        _logger.LogDebug("Invalidated cache for prefix: {Prefix}", JobsPrefix);
    }

    private JobResponse ToJobResponse(JobPost jobPost) => new()
    {
        Id = jobPost.Id,
        Title = jobPost.Title,
        Description = jobPost.Description,
        Requirements = jobPost.Requirements,
        IsActive = jobPost.IsActive
    };
}
