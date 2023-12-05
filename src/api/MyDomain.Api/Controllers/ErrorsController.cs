using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MyDomain.Api.Controllers;

/// <summary>
/// Handle unexpected errors
/// </summary>
public class ErrorsController : ControllerBase
{
    /// <summary>
    /// Display API errors
    /// </summary>
    /// <returns>Error message</returns>
    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

        return Problem(title: exception?.Message);
    }
}