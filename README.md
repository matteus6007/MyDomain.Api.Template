# MyDomain.Api.Template

[![codecov](https://codecov.io/gh/matteus6007/MyDomain.Api.Template/branch/main/graph/badge.svg)](https://codecov.io/gh/matteus6007/MyDomain.Api.Template)

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
  * Database migrations using [Flyway](https://flywaydb.org/)
* AWS resources
  * [Terraform](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/guides/custom-service-endpoints#localstack)
  * [Localstack](https://github.com/localstack/localstack) 

Start environment:

`.\environment.ps1 start`

Stop environment:

`.\environment.ps1 stop`

_Optionally_ you can override the environment variable file:

`.\environment.ps1 start|stop -env_file "{environment}.env"`

## Running the Application

[Health checks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks) can be extended using https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks

_Note: requires local test environment._

### Locally

`dotnet run --project ./src/api/MyDomain.Api`

* Swagger - `http://localhost:5185/swagger/index.html`
* Health checks - `http://localhost:5185/healthchecks-ui`

### Docker

`docker-compose up --build`

* Swagger - `http://localhost:1001/swagger/index.html`
* Health checks - `http://localhost:1001/healthchecks-ui`

## Testing

### Locally

`dotnet test ./src/api`

### Docker

`docker-compose -f docker-compose.test.yml up --build`

## Code Coverage

Managed using [Codecov](https://about.codecov.io/).

To set up the repository you need to create a token `CODECOV_TOKEN` and enable app integration - see https://docs.codecov.com/docs/github-2-getting-a-codecov-account-and-uploading-coverage.
