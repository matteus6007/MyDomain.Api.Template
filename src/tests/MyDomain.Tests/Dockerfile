FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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

ENTRYPOINT ["dotnet", "test", "--logger:\"trx;LogFilePath=./TestResults/test-results.trx;MethodFormat=Class;FailureBodyFormat=Verbose\"", "--configuration:release", "--collect:\"XPlat Code Coverage;Format=opencover\"", "MyDomain.Tests/MyDomain.Tests.csproj"]