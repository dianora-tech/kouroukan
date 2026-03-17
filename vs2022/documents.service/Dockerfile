# =============================================================================
# Kouroukan - Documents Service (.NET 8)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY documents.service/Documents.Service.sln ./
COPY documents.service/src/Documents.Domain/Documents.Domain.csproj src/Documents.Domain/
COPY documents.service/src/Documents.Application/Documents.Application.csproj src/Documents.Application/
COPY documents.service/src/Documents.Infrastructure/Documents.Infrastructure.csproj src/Documents.Infrastructure/
COPY documents.service/src/Documents.Api/Documents.Api.csproj src/Documents.Api/
COPY documents.service/tests/Documents.Tests.Unit/Documents.Tests.Unit.csproj tests/Documents.Tests.Unit/
COPY documents.service/tests/Documents.Tests.Integration/Documents.Tests.Integration.csproj tests/Documents.Tests.Integration/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY documents.service/ .
RUN dotnet publish src/Documents.Api/Documents.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -g 1001 -S kouroukan && adduser -S kouroukan -u 1001 -G kouroukan
COPY --from=build --chown=kouroukan:kouroukan /app/publish .
USER kouroukan

ENV ASPNETCORE_URLS=http://+:5009
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5009

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5009/health || exit 1

ENTRYPOINT ["dotnet", "Documents.Api.dll"]
