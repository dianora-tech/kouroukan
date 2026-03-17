FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY presences.service/src/Presences.Api/Presences.Api.csproj presences.service/src/Presences.Api/
COPY presences.service/src/Presences.Domain/Presences.Domain.csproj presences.service/src/Presences.Domain/
COPY presences.service/src/Presences.Application/Presences.Application.csproj presences.service/src/Presences.Application/
COPY presences.service/src/Presences.Infrastructure/Presences.Infrastructure.csproj presences.service/src/Presences.Infrastructure/
COPY gn-core-libraries/GnDapper/GnDapper.csproj gn-core-libraries/GnDapper/
COPY gn-core-libraries/GnSecurity/GnSecurity.csproj gn-core-libraries/GnSecurity/
COPY gn-core-libraries/GnMessaging/GnMessaging.csproj gn-core-libraries/GnMessaging/
COPY gn-core-libraries/GnValidation/GnValidation.csproj gn-core-libraries/GnValidation/
RUN dotnet restore presences.service/src/Presences.Api/Presences.Api.csproj
COPY . .
RUN dotnet build presences.service/src/Presences.Api/Presences.Api.csproj -c Release -o /app/build
FROM build AS publish
RUN dotnet publish presences.service/src/Presences.Api/Presences.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5004
ENV ASPNETCORE_URLS=http://+:5004
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Presences.Api.dll"]
