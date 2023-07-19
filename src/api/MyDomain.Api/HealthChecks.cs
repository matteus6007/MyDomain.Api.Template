namespace MyDomain.Api;

public static class HealthChecks
{
    public static IHealthChecksBuilder ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var healthChecksBuilder = services.AddHealthChecks();

        if (bool.TryParse(configuration["HealthChecksUI_Enabled"], out var enabled) && enabled)
        {
            services.AddHealthChecksUI(setup =>
            {
                var healthCheckEndpoint = "http://localhost/healthcheck/tests";

#if DEBUG
                healthCheckEndpoint = "/healthcheck/tests";
#endif

                setup.AddHealthCheckEndpoint("API", healthCheckEndpoint);
                setup.SetEvaluationTimeInSeconds(60);
            }).AddInMemoryStorage();
        }

        return healthChecksBuilder;
    }
}