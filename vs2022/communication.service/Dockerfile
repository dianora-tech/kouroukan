# =============================================================================
# Kouroukan - Communication Service (.NET 8)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY communication.service/Communication.Service.sln ./
COPY communication.service/src/Communication.Domain/Communication.Domain.csproj src/Communication.Domain/
COPY communication.service/src/Communication.Application/Communication.Application.csproj src/Communication.Application/
COPY communication.service/src/Communication.Infrastructure/Communication.Infrastructure.csproj src/Communication.Infrastructure/
COPY communication.service/src/Communication.Api/Communication.Api.csproj src/Communication.Api/
COPY communication.service/tests/Communication.Tests.Unit/Communication.Tests.Unit.csproj tests/Communication.Tests.Unit/
COPY communication.service/tests/Communication.Tests.Integration/Communication.Tests.Integration.csproj tests/Communication.Tests.Integration/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY communication.service/ .
RUN dotnet publish src/Communication.Api/Communication.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -g 1001 -S kouroukan && adduser -S kouroukan -u 1001 -G kouroukan
COPY --from=build --chown=kouroukan:kouroukan /app/publish .
USER kouroukan

ENV ASPNETCORE_URLS=http://+:5007
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5007

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5007/health || exit 1

ENTRYPOINT ["dotnet", "Communication.Api.dll"]
