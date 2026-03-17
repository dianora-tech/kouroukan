# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for restore
COPY kouroukan.api/src/kouroukan.api.Gateway/kouroukan.api.Gateway.csproj kouroukan.api/src/kouroukan.api.Gateway/
COPY gn-core-libraries/GnDapper/GnDapper.csproj gn-core-libraries/GnDapper/
COPY gn-core-libraries/GnSecurity/GnSecurity.csproj gn-core-libraries/GnSecurity/
COPY gn-core-libraries/GnMessaging/GnMessaging.csproj gn-core-libraries/GnMessaging/
COPY gn-core-libraries/GnValidation/GnValidation.csproj gn-core-libraries/GnValidation/

# Restore
RUN dotnet restore kouroukan.api/src/kouroukan.api.Gateway/kouroukan.api.Gateway.csproj

# Copy source
COPY . .

# Build
RUN dotnet build kouroukan.api/src/kouroukan.api.Gateway/kouroukan.api.Gateway.csproj -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish kouroukan.api/src/kouroukan.api.Gateway/kouroukan.api.Gateway.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "kouroukan.api.Gateway.dll"]
