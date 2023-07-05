using Amazon.Runtime;
using Amazon.SimpleNotificationService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyDomain.Application;
using MyDomain.Application.Common.Interfaces.Messaging;
using MyDomain.Infrastructure.Messaging.Options;

namespace MyDomain.Infrastructure.Messaging;

public static class DependencyInjection
{
    public static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var snsOptions = configuration.GetSection(SnsOptions.SectionName).Get<SnsOptions>();

        services.AddSingleton(x => snsOptions);

        if (!string.IsNullOrWhiteSpace(snsOptions.ServiceUrlOverride))
        {
            var clientConfig = new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = snsOptions.ServiceUrlOverride,
                UseHttp = true
            };

            services.AddSingleton<IAmazonSimpleNotificationService>(new AmazonSimpleNotificationServiceClient(GetTestCredentials(), clientConfig));
        }
        else
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSimpleNotificationService>();
        }

        services.AddScoped<IMessageEnvelopeBuilder, MessageEnvelopeBuilder>();
        services.AddScoped<IEventPublisher, SnsEventPublisher>();
    }

    private static BasicAWSCredentials GetTestCredentials() => new BasicAWSCredentials("test", "test");
}
