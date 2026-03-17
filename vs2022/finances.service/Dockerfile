# =============================================================================
# Kouroukan - Finances Service (.NET 8)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY finances.service/Finances.Service.sln ./
COPY finances.service/src/Finances.Domain/Finances.Domain.csproj src/Finances.Domain/
COPY finances.service/src/Finances.Application/Finances.Application.csproj src/Finances.Application/
COPY finances.service/src/Finances.Infrastructure/Finances.Infrastructure.csproj src/Finances.Infrastructure/
COPY finances.service/src/Finances.Api/Finances.Api.csproj src/Finances.Api/
COPY finances.service/tests/Finances.Tests.Unit/Finances.Tests.Unit.csproj tests/Finances.Tests.Unit/
COPY finances.service/tests/Finances.Tests.Integration/Finances.Tests.Integration.csproj tests/Finances.Tests.Integration/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY finances.service/ .
RUN dotnet publish src/Finances.Api/Finances.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -g 1001 -S kouroukan && adduser -S kouroukan -u 1001 -G kouroukan
COPY --from=build --chown=kouroukan:kouroukan /app/publish .
USER kouroukan

ENV ASPNETCORE_URLS=http://+:5005
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5005

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5005/health || exit 1

ENTRYPOINT ["dotnet", "Finances.Api.dll"]
