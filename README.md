# MyDomain.Api.Template

[![codecov](https://codecov.io/gh/matteus6007/MyDomain.Api.Template/branch/main/graph/badge.svg)](https://codecov.io/gh/matteus6007/MyDomain.Api.Template) [![tests](https://gist.githubusercontent.com/matteus6007/bcdf0ee6170070c0fcc68059569e76a7/raw/tests.svg)](https://gist.githubusercontent.com/matteus6007/bcdf0ee6170070c0fcc68059569e76a7/raw/tests.svg)

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

## Set up Mock Authentication

### Current JWK

Header:

```
{
  "alg": "RS256",
  "typ": "JWT",
  "kid": "aa9a6015-0f47-436a-a99c-e8afb9810bde"
}
```

Payload:

```
{
    "iss": "http://localhost:8081/identity/",
    "iat": 1621262043,
    "exp": 1905258846,
    "aud": "mydomain-api",
    "sub": "test@example.com",
    "name": "Johnny",
    "last_name": "Rocket",
    "email": "test@example.com",
    "scope": ["mydomain-api:write"]
}
```

*Note: Expiry is set to `2031-01-01`.*

Signature (header):

```
{
  "kty": "RSA",
  "e": "AQAB",
  "kid": "aa9a6015-0f47-436a-a99c-e8afb9810bde",
  "alg": "RS256",
  "n": "h5_3CfgOS_aiTEigdv3LtLIexDrGWFVdnLLYOGOoeLur-gFcN65Ecff2Rt261GAUOTJYCcr0GPhz3wRcBH0r2-aJOYNfgzupo8iL-tjngGb_U8pFeZqSXGoeP4mG8FcN4wnKSoeMYMeLoUZhli3YE1RtjBb17ckUEJaX7q9PzJvubQZnqChWFjxkAf8Fa8ZuzBVqP_2_hm-09Ly7DTRl994DqLC3cchvtFC5wFaq_wABg43cvQn9ipmfx-oWg3uOTmlQjJmnrLbFkTnrSBLDMZCa6IpscphEbatwhAtrxTCfJf0L_kDpuj6scdeIUSj62vdsi_wDWXGz5cQxLLMQyQ"
}
```

Signature (payload):

```
{
  "p": "0iwVIDn_VTBbstpJOXUDmK7uIDPQ8V6l-UcadgFp1NxewfG-hLFX0BGAUvTA114vY-6PiepkPEBsetJRG6gj3b7yZj1M-iWaJGccne59sOLScYkeicTn0MhgglM0DHDU0DRR-DRxHA4O2DTFFy-FUHrGJ6xBr9K6RHIYNly_Zjk",
  "kty": "RSA",
  "q": "pTKc5nc_sONWZtfBKZg-n2_SOMJMvPZlsjct1DNf79PdtJg4MGdLk8eRwtiKVTvGU-gy_3EZwJFxIMC3PIVpfrS2Jiod4iIv1yUzynpZchd9HgZ2N3I2XDBeOYLpYVenySIqkiFlBgGGd5ZoLbixdJpZDOpEATGo68xloSp_fxE",
  "d": "dZkU4cX-z4tJaw_GY6bpDQMtfkTgWxOaGhdROIPhPt6r3hlz1qv6mT_Cgewc5a1xm661e7hJM6WrAwOKrjHC5-fbsMzi8q23CFcKWTXedg3Y8tR-rVSD-DHjjIA5SwiQ9_4zM7CY3gnoXqziTQ_vPcOFQWVfVRF3nnKVxtaoQObmuLwdX7sqLymuCeo7J-1IY79RQFdTURpGwjlw55Z9xdikvrHW-2kOAMst6b5bmEz1WSjsYba9oXEVzKy5F1V0URO4f-01W4ueF-hKHDpdT8vMhRRvpyDUWgQtHmswQf_u0d_BjUEZZDd6KOGVldf_HE_BJcu01B_DtDVh4f77AQ",
  "e": "AQAB",
  "kid": "aa9a6015-0f47-436a-a99c-e8afb9810bde",
  "qi": "B6c_O6KHMqHwx0WLp8bu_A6mRDudojVObDzgfEvp1-q1scL-tT_g4thmICwf0Qn9FaxpKa1Xr_hdV_VrQ9drRs73T0VtkXHpweiH3DKLr6Fx_i0bAFeKzc2R32KLmNCYtQB6VuKwP2CIAwVFmGXmoIZEaB32hjihOyNEjjaAEIs",
  "dp": "O53hiDirYuDKwhsFNlUo5gCI732DQVRVxDYVHXAdMxInluAZ6M3dWNn-Con-wZin3LLo6HgiYqzrmJNcCnpRYi8t1y6ATVypMrZE-c-Su8A7bZU1omLGVwTfy1gKpZTD6SNONRe7Ffgu4clmNsN2D3QkDVDheRxPVeb_UzxP1pE",
  "alg": "RS256",
  "dq": "GTY6P-ofLE3l19hBwKldlVOpj8QHlic8qhBFzEby81UwVumMWcbKAAnLMyN9KTjwMcoUTmvidEM-MrN2w6PQz-egjP6MhQCWsI0a9GHr2L_A9p4h2KBzr2oqVziliZepIcc3HApAssP007d-usufOZ58zL3MFyqPDz2onQoxXyE",
  "n": "h5_3CfgOS_aiTEigdv3LtLIexDrGWFVdnLLYOGOoeLur-gFcN65Ecff2Rt261GAUOTJYCcr0GPhz3wRcBH0r2-aJOYNfgzupo8iL-tjngGb_U8pFeZqSXGoeP4mG8FcN4wnKSoeMYMeLoUZhli3YE1RtjBb17ckUEJaX7q9PzJvubQZnqChWFjxkAf8Fa8ZuzBVqP_2_hm-09Ly7DTRl994DqLC3cchvtFC5wFaq_wABg43cvQn9ipmfx-oWg3uOTmlQjJmnrLbFkTnrSBLDMZCa6IpscphEbatwhAtrxTCfJf0L_kDpuj6scdeIUSj62vdsi_wDWXGz5cQxLLMQyQ"
}
```

You can verify the JWT here https://jwt.io/#debugger.

## Launch Environment

* MySQL
  * Database migrations using [Flyway](https://flywaydb.org/)
* AWS resources
  * [Terraform](https://registry.terraform.io/providers/hashicorp/aws/latest/docs/guides/custom-service-endpoints#localstack)
  * [Localstack](https://github.com/localstack/localstack)
* Mocking using [Wiremock](https://wiremock.org/docs/overview/)

Start environment:

`.\environment.ps1 start`

Stop environment:

`.\environment.ps1 stop`

Optional parameters:

* `-env_file "{environment}.env"` - Override the environment variable file.
* `-detach $true|$false` - Run containers in the background. Default is `$true`.

Navigate to http://localhost:8081/__admin/mappings to display mock mappings.

## Running the Application

[Health checks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks) can be extended using https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks

_Note: requires local test environment._

### Locally

`dotnet run --project ./src/api/MyDomain.Api`

* Swagger - http://localhost:5185/swagger/index.html
* Health checks - http://localhost:5185/healthchecks-ui

### Docker

`docker-compose up --build`

* Swagger - http://localhost:1001/swagger/index.html
* Health checks - http://localhost:1001/healthchecks-ui

## Testing

### Locally

`dotnet test ./src`

### Docker

`docker-compose -f docker-compose.test.yml up --build`

## Code Coverage

Managed using [Codecov](https://about.codecov.io/).

To set up the repository you need to create a token `CODECOV_TOKEN` and enable app integration - see https://docs.codecov.com/docs/github-2-getting-a-codecov-account-and-uploading-coverage.

## Test Status Badge

Uses [append-to-gist](https://github.com/marketplace/actions/append-to-gist) action to update `tests.svg`, requires:

1. Creating new gist with the `gistURL` set in the form `https://gist.githubusercontent.com/{user}/{id}`
2. Personal access toke (PAT) with `gist` permission only
3. GitHub Actions secret `GIST_TOKEN` using above PAT
