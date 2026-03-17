# =============================================================================
# Kouroukan - Evaluations Service (.NET 8)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY evaluations.service/Evaluations.Service.sln ./
COPY evaluations.service/src/Evaluations.Domain/Evaluations.Domain.csproj src/Evaluations.Domain/
COPY evaluations.service/src/Evaluations.Application/Evaluations.Application.csproj src/Evaluations.Application/
COPY evaluations.service/src/Evaluations.Infrastructure/Evaluations.Infrastructure.csproj src/Evaluations.Infrastructure/
COPY evaluations.service/src/Evaluations.Api/Evaluations.Api.csproj src/Evaluations.Api/
COPY evaluations.service/tests/Evaluations.Tests.Unit/Evaluations.Tests.Unit.csproj tests/Evaluations.Tests.Unit/
COPY evaluations.service/tests/Evaluations.Tests.Integration/Evaluations.Tests.Integration.csproj tests/Evaluations.Tests.Integration/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY evaluations.service/ .
RUN dotnet publish src/Evaluations.Api/Evaluations.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -g 1001 -S kouroukan && adduser -S kouroukan -u 1001 -G kouroukan
COPY --from=build --chown=kouroukan:kouroukan /app/publish .
USER kouroukan

ENV ASPNETCORE_URLS=http://+:5003
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5003

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5003/health || exit 1

ENTRYPOINT ["dotnet", "Evaluations.Api.dll"]
