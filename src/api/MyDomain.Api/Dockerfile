FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# restore
COPY ./src/api api
RUN dotnet restore "api/MyDomain.Api/MyDomain.Api.csproj"

# build
WORKDIR "/src/api/MyDomain.Api"
RUN dotnet build "MyDomain.Api.csproj" -c Release --no-restore -o /app/build

# test stage -- exposes optional entrypoint
# target entrypoint with: docker build --target test
FROM build AS test
WORKDIR /src/tests
COPY ./src/tests/ .

ENTRYPOINT ["dotnet", "test", "--logger:\"trx;LogFilePath=./TestResults/test-results.trx;MethodFormat=Class;FailureBodyFormat=Verbose\"", "--configuration:release", "/p:CollectCoverage=true", "/p:CoverletOutputFormat=cobertura", "/p:CoverletOutput=./TestResults/", "MyDomain.Tests/MyDomain.Tests.csproj"]

# publish
FROM build AS publish
ARG BUILD_VERSION

RUN dotnet publish "MyDomain.Api.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyDomain.Api.dll"]