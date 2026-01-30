using JobBoard.Identity.Domain.Abstractions.Services;
using JobBoard.Identity.Domain.Requests.Users;
using JobBoard.Identity.Domain.Response;
using JobBoard.Identity.Domain.Response.Users;
using JobBoard.Shared.ApiResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Identity.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [Authorize(Roles =  "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        await _userService.Delete(id, cancellationToken);
        return Ok(new ApiSuccessResponse("User successfully deleted"));
    }
    
    [Authorize(Roles =  "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id,[FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        await _userService.Update(id,request, cancellationToken);
        return Ok(new ApiSuccessResponse("User successfully updated"));
    }
    
    [HttpGet("batch")]
    public async Task<IActionResult> GetBatch([FromQuery] GetUsersBatchRequest request, CancellationToken ct)
    {
        var result = await _userService.GetUsersBatch(request, ct);
        
        return Ok(new ApiSuccessResponse<ResponseList<UsersBatchResponse>>(result, "Users fetched"));
    }
}