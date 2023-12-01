FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# restore
COPY ./src/api api
RUN dotnet restore "api/MyDomain.Api/MyDomain.Api.csproj"

# build
WORKDIR "/src/api/MyDomain.Api"
RUN dotnet build "MyDomain.Api.csproj" -c Release --no-restore -o /app/build

# publish
FROM build AS publish
ARG BUILD_VERSION

RUN dotnet publish "MyDomain.Api.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyDomain.Api.dll"]