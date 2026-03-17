# =============================================================================
# Kouroukan Cache Service (.NET 8 - MemoryCache L1 + Redis L2 + Quartz.NET)
# =============================================================================
# Stage 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY gn-cache/gn-cache.sln ./
COPY gn-cache/gn-cache.Domain/gn-cache.Domain.csproj gn-cache.Domain/
COPY gn-cache/gn-cache.Application/gn-cache.Application.csproj gn-cache.Application/
COPY gn-cache/gn-cache.Infrastructure/gn-cache.Infrastructure.csproj gn-cache.Infrastructure/
COPY gn-cache/gn-cache/gn-cache.csproj gn-cache/
COPY gn-cache/gn-cache.Tests/gn-cache.Tests.csproj gn-cache.Tests/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY gn-cache/ .

RUN dotnet publish gn-cache/gn-cache.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

WORKDIR /app

RUN addgroup -g 1001 -S kouroukan && \
    adduser -S kouroukan -u 1001 -G kouroukan

COPY --from=build --chown=kouroukan:kouroukan /app/publish .

USER kouroukan

ENV ASPNETCORE_URLS=http://+:5100
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5100

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5100/health || exit 1

ENTRYPOINT ["dotnet", "gn-cache.dll"]
