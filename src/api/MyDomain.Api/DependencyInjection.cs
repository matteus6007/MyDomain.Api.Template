using Asp.Versioning;

using FluentValidation;
using FluentValidation.AspNetCore;

using IdentityModel;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

using MyDomain.Api.Authorization;
using MyDomain.Api.Common.Mapping;
using MyDomain.Api.Filters;
using MyDomain.Api.Options;

namespace MyDomain.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddMappings();
        services.AddProblemDetails();
        services.ConfigureOptions<AssemblyOptionsProvider>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        var defaultApiVersion = new ApiVersion(1, 0);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.DocumentFilter<PathLowercaseDocumentFilter>();
            options.OperationFilter<AuthOperationFilter>();
            options.SwaggerDoc("v1", new() { Title = "MyDomain API", Version = "v1" });

            var filePath = Path.Combine(
                AppContext.BaseDirectory,
                $"{typeof(Program).Assembly.GetName().Name}.xml");

            options.IncludeXmlComments(filePath);
        });
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = defaultApiVersion;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            options.SubstituteApiVersionInUrl = true;
            options.GroupNameFormat = "'v'V";
            options.DefaultApiVersion = defaultApiVersion;
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        return services;
    }

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var identityOptions = configuration.GetSection(IdentityOptions.SectionName).Get<IdentityOptions>() ?? new IdentityOptions();

        var authenticationBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        authenticationBuilder.AddJwtBearer(options =>
        {
            options.Authority = identityOptions.Issuer;
            options.Audience = identityOptions.Audience;
            options.RequireHttpsMetadata = identityOptions.RequireHttpsMetadata;
        });

        return services;
    }

    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            options.AddPolicy(Policies.Read, policy => policy.RequireClaim(JwtClaimTypes.Scope, Scopes.Read, Scopes.Write));
            options.AddPolicy(Policies.Write, policy => policy.RequireClaim(JwtClaimTypes.Scope, Scopes.Write));
        });

        return services;
    }
}