# =============================================================================
# Kouroukan - Pedagogie Service (.NET 8)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY pedagogie.service/Pedagogie.Service.sln ./
COPY pedagogie.service/src/Pedagogie.Domain/Pedagogie.Domain.csproj src/Pedagogie.Domain/
COPY pedagogie.service/src/Pedagogie.Application/Pedagogie.Application.csproj src/Pedagogie.Application/
COPY pedagogie.service/src/Pedagogie.Infrastructure/Pedagogie.Infrastructure.csproj src/Pedagogie.Infrastructure/
COPY pedagogie.service/src/Pedagogie.Api/Pedagogie.Api.csproj src/Pedagogie.Api/
COPY pedagogie.service/tests/Pedagogie.Tests.Unit/Pedagogie.Tests.Unit.csproj tests/Pedagogie.Tests.Unit/
COPY pedagogie.service/tests/Pedagogie.Tests.Integration/Pedagogie.Tests.Integration.csproj tests/Pedagogie.Tests.Integration/

COPY gn-core-libraries/ /gn-core-libraries/
RUN dotnet restore

COPY pedagogie.service/ .
RUN dotnet publish src/Pedagogie.Api/Pedagogie.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
RUN addgroup -g 1001 -S kouroukan && adduser -S kouroukan -u 1001 -G kouroukan
COPY --from=build --chown=kouroukan:kouroukan /app/publish .
USER kouroukan

ENV ASPNETCORE_URLS=http://+:5002
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5002

HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:5002/health || exit 1

ENTRYPOINT ["dotnet", "Pedagogie.Api.dll"]
