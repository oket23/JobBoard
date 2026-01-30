using JobBoard.Recruitment.Domain.Abstractions.Services;
using JobBoard.Recruitment.Domain.Requests.Jobs;
using JobBoard.Recruitment.Domain.Response;
using JobBoard.Recruitment.Domain.Response.Jobs;
using JobBoard.Shared.ApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Recruitment.Api.Controllers;

[ApiController]
[Route("api/v1/jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobsServices _jobsService;

    public JobsController(IJobsServices jobsService)
    {
        _jobsService = jobsService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateJobRequest request, CancellationToken ct)
    {
        await _jobsService.Create(request, ct);
        return Ok(new ApiSuccessResponse("Job created successfully"));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] JobRequest request, CancellationToken ct)
    {
        var result = await _jobsService.GetAll(request, ct);
        return Ok(new ApiSuccessResponse<ResponseList<JobResponse>>(result, "Jobs retrieved"));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateJobRequest request, CancellationToken ct)
    {
        await _jobsService.Update(id, request, ct);
        return Ok(new ApiSuccessResponse("Job updated successfully"));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        await _jobsService.Delete(id, ct);
        return Ok(new ApiSuccessResponse("Job deleted successfully"));
    }
}