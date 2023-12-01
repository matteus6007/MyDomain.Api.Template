using Microsoft.Extensions.DependencyInjection;

using MyDomain.Application.Common;
using MyDomain.Application.Common.Interfaces;

namespace MyDomain.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}