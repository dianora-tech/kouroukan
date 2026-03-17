# =============================================================================
# Kouroukan - BDE Service (.NET 8)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY bde.service/Bde.Service.sln ./
COPY bde.service/src/Bde.Domain/Bde.Domain.csproj src/Bde.Domain/
COPY bde.service/src/Bde.Application/Bde.Application.csproj src/Bde.Application/
COPY bde.service/src/Bde.Infrastructure/Bde.Infrastructure.csproj src/Bde.Infrastructure/
COPY bde.service/src/Bde.Api/Bde.Api.csproj src/Bde.Api/
COPY bde.service/tests/Bde.Tests.Unit/Bde.Tests.Unit.csproj tests/Bde.Tests.Unit/
COPY bde.service/tests/Bde.Tests.Integration/Bde.Tests.Integration.csproj tests/Bde.Tests.Integration/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY bde.service/ .
RUN dotnet publish src/Bde.Api/Bde.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -g 1001 -S kouroukan && adduser -S kouroukan -u 1001 -G kouroukan
COPY --from=build --chown=kouroukan:kouroukan /app/publish .
USER kouroukan

ENV ASPNETCORE_URLS=http://+:5008
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5008

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5008/health || exit 1

ENTRYPOINT ["dotnet", "Bde.Api.dll"]
