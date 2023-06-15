namespace MyDomain.Api;

public class LogUnhandledExceptionsMiddleware 
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LogUnhandledExceptionsMiddleware> _logger;

    public LogUnhandledExceptionsMiddleware(RequestDelegate next, ILogger<LogUnhandledExceptionsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unhandled exception was thrown by the application", ex);
            throw;
        }
    }    
}