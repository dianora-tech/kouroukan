# =============================================================================
# Kouroukan - Services Premium Service (.NET 8)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY services-premium.service/ServicesPremium.Service.sln ./
COPY services-premium.service/src/ServicesPremium.Domain/ServicesPremium.Domain.csproj src/ServicesPremium.Domain/
COPY services-premium.service/src/ServicesPremium.Application/ServicesPremium.Application.csproj src/ServicesPremium.Application/
COPY services-premium.service/src/ServicesPremium.Infrastructure/ServicesPremium.Infrastructure.csproj src/ServicesPremium.Infrastructure/
COPY services-premium.service/src/ServicesPremium.Api/ServicesPremium.Api.csproj src/ServicesPremium.Api/
COPY services-premium.service/tests/ServicesPremium.Tests.Unit/ServicesPremium.Tests.Unit.csproj tests/ServicesPremium.Tests.Unit/
COPY services-premium.service/tests/ServicesPremium.Tests.Integration/ServicesPremium.Tests.Integration.csproj tests/ServicesPremium.Tests.Integration/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY services-premium.service/ .
RUN dotnet publish src/ServicesPremium.Api/ServicesPremium.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -g 1001 -S kouroukan && adduser -S kouroukan -u 1001 -G kouroukan
COPY --from=build --chown=kouroukan:kouroukan /app/publish .
USER kouroukan

ENV ASPNETCORE_URLS=http://+:5010
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5010

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5010/health || exit 1

ENTRYPOINT ["dotnet", "ServicesPremium.Api.dll"]
