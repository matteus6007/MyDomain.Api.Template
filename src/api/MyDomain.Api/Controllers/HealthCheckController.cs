using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyDomain.Api.Controllers;

/// <summary>
/// Check health of API
/// </summary>
[ApiController]
[Route("v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
public class HealthCheckController : ControllerBase
{
        /// <summary>
        /// Get health of MyDomain API
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }    
}