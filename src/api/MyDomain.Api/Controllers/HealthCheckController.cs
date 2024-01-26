using System.Net;

using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MyDomain.Contracts.Models.V1;

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
    /// <returns><see cref="HealthCheckDto"/></returns>
    /// <response code="200">HealthCheck returned</response>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(HealthCheckDto), (int)HttpStatusCode.OK)]
    public IActionResult Get()
    {
        return Ok(new HealthCheckDto());
    }
}