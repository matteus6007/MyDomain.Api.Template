# MyDomain.Api.Template
Boilerplate template for creating a new API

## Linting

Performed on `pre-commit` using [Husky.Net](https://alirezanet.github.io/Husky.Net/).

The linting of the codebase is done using `dotnet format` which is included within the dotnet SDK, from version `6.0`.

_Note: for new repositories, as dotnet format will only run against changed files, an 'Initial commit' needs to be present to allow Husky to determine which files are staged._

## Logging

Structured logging using [Serilog](https://serilog.net/).

Example configuration in `appsettings.json`:

```json
"Serilog": {
    "Using": ["Serilog.Sinks.Console"],
    "MinimumLevel": {
        "Default": "Information",
        "Override": {
            "Microsoft": "Warning",
            "Microsoft.AspNetCore": "Error",
            "System": "Warning"
        }
    },
    "WriteTo": [
        { "Name": "Console" }
    ],
    "Enrich": ["FromLogContext"]
}
```

## Launch Environment

* MySQL
* Database migrations

Start environment:

`.\environment.ps1 start`

Stop environment:

`.\environment.ps1 stop`

## Running the Application

Locally:

`dotnet run --project ./src/api/MyDomain.Api`

_Note: requires local test environment._

### Testing

Locally:

`dotnet test ./src/api`

Using `docker-compose` _(to replicate pipeline)_:

`docker-compose -f docker-compose.test.yml up --build`

### Swagger

Navigate to `http://localhost:5185/swagger/index.html`

### Health Checks

https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks and https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks

Navigate to `http://localhost:5185/healthchecks-ui` to view health of API dependencies.