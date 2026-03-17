# =============================================================================
# Kouroukan - Support Service (.NET 8) — avec Ollama (IA generative)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY support.service/Support.Service.sln ./
COPY support.service/src/Support.Domain/Support.Domain.csproj src/Support.Domain/
COPY support.service/src/Support.Application/Support.Application.csproj src/Support.Application/
COPY support.service/src/Support.Infrastructure/Support.Infrastructure.csproj src/Support.Infrastructure/
COPY support.service/src/Support.Api/Support.Api.csproj src/Support.Api/
COPY support.service/tests/Support.Tests.Unit/Support.Tests.Unit.csproj tests/Support.Tests.Unit/
COPY support.service/tests/Support.Tests.Integration/Support.Tests.Integration.csproj tests/Support.Tests.Integration/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY support.service/ .
RUN dotnet publish src/Support.Api/Support.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -g 1001 -S kouroukan && adduser -S kouroukan -u 1001 -G kouroukan
COPY --from=build --chown=kouroukan:kouroukan /app/publish .
USER kouroukan

ENV ASPNETCORE_URLS=http://+:5011
ENV ASPNETCORE_ENVIRONMENT=Production
ENV Ollama__BaseUrl=http://ollama:11434
ENV Ollama__Model=mistral

EXPOSE 5011

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5011/health || exit 1

ENTRYPOINT ["dotnet", "Support.Api.dll"]
