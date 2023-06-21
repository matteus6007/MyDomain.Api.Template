using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyDomain.Api.Controllers;

/// <summary>
/// Check health of API
/// </summary>
[ApiVersion("1.0")]
public class HealthCheckController : ApiController
{
    /// <summary>
    /// Get health of MyDomain API
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "Healthy" });
    }
}