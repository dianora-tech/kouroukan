FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY inscriptions.service/src/Inscriptions.Api/Inscriptions.Api.csproj inscriptions.service/src/Inscriptions.Api/
COPY inscriptions.service/src/Inscriptions.Domain/Inscriptions.Domain.csproj inscriptions.service/src/Inscriptions.Domain/
COPY inscriptions.service/src/Inscriptions.Application/Inscriptions.Application.csproj inscriptions.service/src/Inscriptions.Application/
COPY inscriptions.service/src/Inscriptions.Infrastructure/Inscriptions.Infrastructure.csproj inscriptions.service/src/Inscriptions.Infrastructure/
COPY gn-core-libraries/GnDapper/GnDapper.csproj gn-core-libraries/GnDapper/
COPY gn-core-libraries/GnSecurity/GnSecurity.csproj gn-core-libraries/GnSecurity/
COPY gn-core-libraries/GnMessaging/GnMessaging.csproj gn-core-libraries/GnMessaging/
COPY gn-core-libraries/GnValidation/GnValidation.csproj gn-core-libraries/GnValidation/
RUN dotnet restore inscriptions.service/src/Inscriptions.Api/Inscriptions.Api.csproj
COPY . .
RUN dotnet build inscriptions.service/src/Inscriptions.Api/Inscriptions.Api.csproj -c Release -o /app/build
FROM build AS publish
RUN dotnet publish inscriptions.service/src/Inscriptions.Api/Inscriptions.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Inscriptions.Api.dll"]
