using Microsoft.AspNetCore.Mvc;

namespace MyDomain.Api.Controllers;

public class ErrorsController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error()
    {
        return Problem();
    }
}