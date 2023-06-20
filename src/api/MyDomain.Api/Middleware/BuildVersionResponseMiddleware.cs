using Microsoft.Extensions.Options;

using MyDomain.Api.Options;

namespace MyDomain.Api.Middleware;

public class BuildVersionResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AssemblyOptions _options;

    public BuildVersionResponseMiddleware(RequestDelegate next, IOptions<AssemblyOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public Task Invoke(HttpContext context)
    {
        context.Response.Headers.Add("X-Build-Version", _options.Version);

        return _next(context);
    }
}