using System.Security.Claims;
using JobBoard.Recruitment.Domain.Abstractions.Services;
using JobBoard.Recruitment.Domain.Requests.Applications;
using JobBoard.Recruitment.Domain.Response;
using JobBoard.Recruitment.Domain.Response.Applications;
using JobBoard.Shared.ApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Recruitment.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/applications")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationsService _applicationsService;

    public ApplicationsController(IApplicationsService applicationsService)
    {
        _applicationsService = applicationsService;
    }

    [HttpPost("{jobId:int}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Submit([FromRoute] int jobId, [FromBody] CreateApplicationRequest request, CancellationToken ct)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                           
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized(new ApiErrorResponse("Unauthorized", "Invalid user ID in token"));
        }
        
        if (string.IsNullOrWhiteSpace(userIdString))
        {
            return Unauthorized(new ApiErrorResponse("Unauthorized", "Invalid user ID in token"));
        }
        
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
        var userName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? User.FindFirst("given_name")?.Value;
        
        var appRequest = new CreateApplicationRequest
        {
            CoverLetter = request.CoverLetter,
            FirstName = userName,
            Email = userEmail,
            UserId = userId,
            JobId = jobId
        };

        await _applicationsService.Create(appRequest, ct);
        
        return Ok(new ApiSuccessResponse("Application submitted successfully"));
    }

    [HttpGet("user")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetMyApplications([FromQuery] UserApplicationRequest request,CancellationToken ct)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized(new ApiErrorResponse("Unauthorized", "Invalid user ID in token"));
        }

        var result = await _applicationsService.GetUserApplications(userId, request, ct);
        return Ok(new ApiSuccessResponse<ResponseList<UserApplicationResponse>>(result, "User applications retrieved"));
    }
    
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] ChangeApplicationStatusRequest request, CancellationToken ct)
    {
         await _applicationsService.ChangeStatus(id, request, ct);
         return Ok(new ApiSuccessResponse("Application status updated"));
    }
}