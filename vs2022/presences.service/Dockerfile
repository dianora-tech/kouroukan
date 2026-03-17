# =============================================================================
# Kouroukan - Presences Service (.NET 8)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY presences.service/Presences.Service.sln ./
COPY presences.service/src/Presences.Domain/Presences.Domain.csproj src/Presences.Domain/
COPY presences.service/src/Presences.Application/Presences.Application.csproj src/Presences.Application/
COPY presences.service/src/Presences.Infrastructure/Presences.Infrastructure.csproj src/Presences.Infrastructure/
COPY presences.service/src/Presences.Api/Presences.Api.csproj src/Presences.Api/
COPY presences.service/tests/Presences.Tests.Unit/Presences.Tests.Unit.csproj tests/Presences.Tests.Unit/
COPY presences.service/tests/Presences.Tests.Integration/Presences.Tests.Integration.csproj tests/Presences.Tests.Integration/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY presences.service/ .
RUN dotnet publish src/Presences.Api/Presences.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -g 1001 -S kouroukan && adduser -S kouroukan -u 1001 -G kouroukan
COPY --from=build --chown=kouroukan:kouroukan /app/publish .
USER kouroukan

ENV ASPNETCORE_URLS=http://+:5004
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5004

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5004/health || exit 1

ENTRYPOINT ["dotnet", "Presences.Api.dll"]
