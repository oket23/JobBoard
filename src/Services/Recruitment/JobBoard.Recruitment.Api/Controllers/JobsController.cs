using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Recruitment.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/jobs")]
public class JobsController : ControllerBase
{
    
}