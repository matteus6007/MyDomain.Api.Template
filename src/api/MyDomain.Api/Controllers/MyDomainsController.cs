using System.Net;

using ErrorOr;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MyDomain.Application.Services.Commands.Common;
using MyDomain.Application.Services.Commands.CreateMyDomain;
using MyDomain.Contracts.Models.V1;
using MyDomain.Contracts.Requests.V1;

namespace MyDomain.Api.Controllers;

/// <summary>
/// Manage MyDomain
/// </summary>
[ApiController]
[Route("v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
public class MyDomainsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MyDomainsController(IMediator mediator)
    {
        _mediator = mediator;
    }

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
        var response = new MyDomainDto(id, "Test name", "Test description", DateTime.UtcNow, DateTime.UtcNow);

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
    public async Task<IActionResult>  Create(CreateMyDomainRequest request)
    {
        var command = new CreateMyDomainCommand(request.Name, request.Description ?? string.Empty);

        ErrorOr<MyDomainResult> response = await _mediator.Send(command);

        // TODO: add contract mapper
        return response.Match(
            dto => CreatedAtAction(nameof(GetById), new { dto.Id }, dto),
            error => StatusCode((int)HttpStatusCode.InternalServerError, new { error = "Something went wrong" }));
    }

    /// <summary>
    /// Update existing MyDomain
    /// </summary>
    /// <param name="id">MyDomain ID</param>
    /// <param name="request">Update MyDomain request</param>
    /// <returns><see cref="MyDomainDto" /></returns>
    /// <response code="200">MyDomain returned</response>
    /// <response code="404">MyDomain not found</response>    
    [HttpPut]
    [Route("{id:guid}")]
    [ProducesResponseType(typeof(MyDomainDto), (int)HttpStatusCode.OK)]
    public IActionResult Update(Guid id, UpdateMyDomainRequest request)
    {
        // TODO: Check if MyDomain exists

        // TODO: Move to Application
        var response = new MyDomainDto(id, request.Name, request.Description, DateTime.UtcNow, DateTime.UtcNow);

        return Ok(response);
    }
}