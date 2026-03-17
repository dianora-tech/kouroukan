# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY gn-cache/gn-cache.csproj gn-cache/
COPY gn-cache.Domain/gn-cache.Domain.csproj gn-cache.Domain/
COPY gn-cache.Application/gn-cache.Application.csproj gn-cache.Application/
COPY gn-cache.Infrastructure/gn-cache.Infrastructure.csproj gn-cache.Infrastructure/

# Restore
RUN dotnet restore gn-cache/gn-cache.csproj

# Copy source
COPY . .

# Build
RUN dotnet build gn-cache/gn-cache.csproj -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish gn-cache/gn-cache.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

# Copy seed data
COPY data/seed/ data/seed/

EXPOSE 5100
ENV ASPNETCORE_URLS=http://+:5100

ENTRYPOINT ["dotnet", "gn-cache.dll"]
