using System.Net;

using Mapster;

using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using MyDomain.Application.Services.Commands.CreateMyDomain;
using MyDomain.Application.Services.Commands.UpdateMyDomain;
using MyDomain.Application.Services.Queries;
using MyDomain.Contracts.Models.V1;
using MyDomain.Contracts.Requests.V1;

namespace MyDomain.Api.Controllers;

/// <summary>
/// Manage MyDomain
/// </summary>
[ApiVersion("1.0")]
public class MyDomainsController : ApiController
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public MyDomainsController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
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
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetMyDomainByIdQuery(id);

        var response = await _mediator.Send(query);

        return response.Match(
            result => Ok(_mapper.Map<MyDomainDto>(result)),
            Problem);
    }

    /// <summary>
    /// Create new MyDomain
    /// </summary>
    /// <param name="request">Create new MyDomain request</param>
    /// <returns><see cref="MyDomainDto" /></returns>
    /// <response code="201">MyDomain created</response>
    [HttpPost]
    [ProducesResponseType(typeof(MyDomainDto), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create(CreateMyDomainRequest request)
    {
        var command = _mapper.Map<CreateMyDomainCommand>(request);

        var response = await _mediator.Send(command);

        return response.Match(
            result => CreatedAtAction(nameof(GetById), new { result.Id }, _mapper.Map<MyDomainDto>(result)),
            Problem);
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
    public async Task<IActionResult> Update(Guid id, UpdateMyDomainRequest request)
    {
        var command = (request, id).Adapt<UpdateMyDomainCommand>();

        var response = await _mediator.Send(command);

        return response.Match(
            result => Ok(_mapper.Map<MyDomainDto>(result)),
            Problem);
    }
}