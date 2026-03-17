# =============================================================================
# Kouroukan API Gateway (.NET 8)
# =============================================================================
# Stage 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copier les fichiers solution et projets pour optimiser le cache des layers
COPY kouroukan.api/kouroukan.api.sln ./
COPY kouroukan.api/src/kouroukan.api.Gateway/kouroukan.api.Gateway.csproj src/kouroukan.api.Gateway/
COPY kouroukan.api/tests/kouroukan.api.Gateway.Tests/kouroukan.api.Gateway.Tests.csproj tests/kouroukan.api.Gateway.Tests/

# Restaurer les dependances (NuGet packages locaux si necessaire)
COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

# Copier le reste du code source
COPY kouroukan.api/ .

# Publier en mode Release
RUN dotnet publish src/kouroukan.api.Gateway/kouroukan.api.Gateway.csproj \
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

ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 5000

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5000/ready || exit 1

ENTRYPOINT ["dotnet", "kouroukan.api.Gateway.dll"]
