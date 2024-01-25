FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_VERSION=1.0.0
ARG HUSKY=0
ARG SSL_CERT_DIR=/etc/ssl/certs/
ARG CERT_FILE_PATH
ARG CERT_FILE

#Add the CA Certificate to the container
ADD "*$CERT_FILE_PATH/$CERT_FILE" /src/tests/**/Dockerfile /tmp/
RUN if [ -f "/tmp/$CERT_FILE" ] ; then cp /tmp/$CERT_FILE $SSL_CERT_DIR ; fi
RUN update-ca-certificates

WORKDIR /src

# restore
COPY ./src/api api
RUN dotnet restore "api/MyDomain.Api/MyDomain.Api.csproj"

# build
WORKDIR "/src/api/MyDomain.Api"
RUN dotnet build "MyDomain.Api.csproj" -c Release --no-restore -o /app/build /p:Version=$BUILD_VERSION

# publish
FROM build AS publish
ARG BUILD_VERSION

RUN dotnet publish "MyDomain.Api.csproj" -c Release --no-restore -o /app/publish /p:Version=$BUILD_VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyDomain.Api.dll"]