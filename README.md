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

## Running Tests

Locally:

`dotnet test ./src/api`

Using `docker-compose` _(same as pipeling)_:

`docker-compose -f docker-compose.test.yml up --build`
