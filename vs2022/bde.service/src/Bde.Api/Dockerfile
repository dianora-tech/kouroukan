FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY bde.service/src/Bde.Api/Bde.Api.csproj bde.service/src/Bde.Api/
COPY bde.service/src/Bde.Domain/Bde.Domain.csproj bde.service/src/Bde.Domain/
COPY bde.service/src/Bde.Application/Bde.Application.csproj bde.service/src/Bde.Application/
COPY bde.service/src/Bde.Infrastructure/Bde.Infrastructure.csproj bde.service/src/Bde.Infrastructure/
COPY gn-core-libraries/GnDapper/GnDapper.csproj gn-core-libraries/GnDapper/
COPY gn-core-libraries/GnSecurity/GnSecurity.csproj gn-core-libraries/GnSecurity/
COPY gn-core-libraries/GnMessaging/GnMessaging.csproj gn-core-libraries/GnMessaging/
COPY gn-core-libraries/GnValidation/GnValidation.csproj gn-core-libraries/GnValidation/
RUN dotnet restore bde.service/src/Bde.Api/Bde.Api.csproj
COPY . .
RUN dotnet build bde.service/src/Bde.Api/Bde.Api.csproj -c Release -o /app/build
FROM build AS publish
RUN dotnet publish bde.service/src/Bde.Api/Bde.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5008
ENV ASPNETCORE_URLS=http://+:5008
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Bde.Api.dll"]
