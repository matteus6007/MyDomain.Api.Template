using System.Net;

using Microsoft.AspNetCore.Mvc;
using MyDomain.Contracts.Models.V1;
using MyDomain.Contracts.Requests.V1;

namespace MyDomain.Api.Controllers.V1;

/// <summary>
/// Manage MyDomain
/// </summary>
[ApiController]
[Route("v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
public class MyDomainsController : ControllerBase
{
    /// <summary>
    /// Get MyDomain by ID
    /// </summary>
    /// <param name="id">MyDomain ID</param>
    /// <returns><see cref="MyDomainDto" /></returns>
    /// <response code="200">MyDomain returned</response>
    /// <response code="404">MyDomain not found</response>
    [HttpGet]
    [Route("{id:guid}")]
    [ProducesResponseType(typeof(MyDomainDto), (int)HttpStatusCode.OK)]
    public IActionResult GetById(Guid id)
    {
        //TODO: Move to Application
        var response = new MyDomainDto(id, "Test name", "Test description", DateTime.UtcNow, null);

        return Ok(response);
    }

    /// <summary>
    /// Create new MyDomain
    /// </summary>
    /// <param name="request">Create new MyDomain request</param>
    /// <returns><see cref="MyDomainDto" /></returns>
    /// <response code="201">MyDomain created</response>
    [HttpPost]
    [ProducesResponseType(typeof(MyDomainDto), (int)HttpStatusCode.Created)]
    public IActionResult Create(CreateMyDomainRequest request)
    {
        //TODO: Move to Application
        var response = new MyDomainDto(Guid.NewGuid(), request.name, request.description, DateTime.UtcNow, null);

        return CreatedAtAction(nameof(GetById), new {response.id}, response);
    }
}