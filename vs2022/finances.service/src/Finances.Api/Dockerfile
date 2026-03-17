FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 5005

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copier les fichiers projet pour le restore
COPY ["src/Finances.Api/Finances.Api.csproj", "src/Finances.Api/"]
COPY ["src/Finances.Domain/Finances.Domain.csproj", "src/Finances.Domain/"]
COPY ["src/Finances.Application/Finances.Application.csproj", "src/Finances.Application/"]
COPY ["src/Finances.Infrastructure/Finances.Infrastructure.csproj", "src/Finances.Infrastructure/"]

# Copier les projets gn-core-libraries
COPY ["../gn-core-libraries/GnDapper/GnDapper.csproj", "../gn-core-libraries/GnDapper/"]
COPY ["../gn-core-libraries/GnSecurity/GnSecurity.csproj", "../gn-core-libraries/GnSecurity/"]
COPY ["../gn-core-libraries/GnMessaging/GnMessaging.csproj", "../gn-core-libraries/GnMessaging/"]
COPY ["../gn-core-libraries/GnValidation/GnValidation.csproj", "../gn-core-libraries/GnValidation/"]

RUN dotnet restore "src/Finances.Api/Finances.Api.csproj"

COPY . .
WORKDIR "/src/src/Finances.Api"
RUN dotnet build "Finances.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Finances.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5005
ENTRYPOINT ["dotnet", "Finances.Api.dll"]
