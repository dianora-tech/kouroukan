# =============================================================================
# Kouroukan - Inscriptions Service (.NET 8)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY inscriptions.service/Inscriptions.Service.sln ./
COPY inscriptions.service/src/Inscriptions.Domain/Inscriptions.Domain.csproj src/Inscriptions.Domain/
COPY inscriptions.service/src/Inscriptions.Application/Inscriptions.Application.csproj src/Inscriptions.Application/
COPY inscriptions.service/src/Inscriptions.Infrastructure/Inscriptions.Infrastructure.csproj src/Inscriptions.Infrastructure/
COPY inscriptions.service/src/Inscriptions.Api/Inscriptions.Api.csproj src/Inscriptions.Api/
COPY inscriptions.service/tests/Inscriptions.Tests.Unit/Inscriptions.Tests.Unit.csproj tests/Inscriptions.Tests.Unit/
COPY inscriptions.service/tests/Inscriptions.Tests.Integration/Inscriptions.Tests.Integration.csproj tests/Inscriptions.Tests.Integration/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY inscriptions.service/ .
RUN dotnet publish src/Inscriptions.Api/Inscriptions.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -g 1001 -S kouroukan && adduser -S kouroukan -u 1001 -G kouroukan
COPY --from=build --chown=kouroukan:kouroukan /app/publish .
USER kouroukan

ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5001

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5001/health || exit 1

ENTRYPOINT ["dotnet", "Inscriptions.Api.dll"]
