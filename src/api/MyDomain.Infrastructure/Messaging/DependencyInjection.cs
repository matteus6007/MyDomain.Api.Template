using Amazon.SimpleNotificationService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyDomain.Application.Common.Interfaces.Messaging;
using MyDomain.Infrastructure.Extensions;
using MyDomain.Infrastructure.Messaging.Options;

namespace MyDomain.Infrastructure.Messaging;

public static class DependencyInjection
{
    public static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var snsOptions = configuration.GetSection(SnsOptions.SectionName).Get<SnsOptions>();

#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
        services.AddSingleton(snsOptions);
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.

        if (!string.IsNullOrWhiteSpace(snsOptions?.ServiceUrlOverride))
        {
            var clientConfig = new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = snsOptions.ServiceUrlOverride,
                UseHttp = true
            };

            services.AddSingleton<IAmazonSimpleNotificationService>(new AmazonSimpleNotificationServiceClient(AwsExtensions.GetTestCredentials(), clientConfig));
        }
        else
        {
            services.AddAWSService<IAmazonSimpleNotificationService>();
        }

        services.AddScoped<IMessageEnvelopeBuilder, MessageEnvelopeBuilder>();
        services.AddScoped<IEventPublisher, SnsEventPublisher>();
    }
}