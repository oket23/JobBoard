using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Identity.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/applications")]
public class ApplicationsController : ControllerBase
{
    
}