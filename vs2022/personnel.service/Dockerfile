# =============================================================================
# Kouroukan - Personnel Service (.NET 8)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY personnel.service/Personnel.Service.sln ./
COPY personnel.service/src/Personnel.Domain/Personnel.Domain.csproj src/Personnel.Domain/
COPY personnel.service/src/Personnel.Application/Personnel.Application.csproj src/Personnel.Application/
COPY personnel.service/src/Personnel.Infrastructure/Personnel.Infrastructure.csproj src/Personnel.Infrastructure/
COPY personnel.service/src/Personnel.Api/Personnel.Api.csproj src/Personnel.Api/
COPY personnel.service/tests/Personnel.Tests.Unit/Personnel.Tests.Unit.csproj tests/Personnel.Tests.Unit/
COPY personnel.service/tests/Personnel.Tests.Integration/Personnel.Tests.Integration.csproj tests/Personnel.Tests.Integration/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY personnel.service/ .
RUN dotnet publish src/Personnel.Api/Personnel.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -g 1001 -S kouroukan && adduser -S kouroukan -u 1001 -G kouroukan
COPY --from=build --chown=kouroukan:kouroukan /app/publish .
USER kouroukan

ENV ASPNETCORE_URLS=http://+:5006
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5006

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5006/health || exit 1

ENTRYPOINT ["dotnet", "Personnel.Api.dll"]
