FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY communication.service/src/Communication.Api/Communication.Api.csproj communication.service/src/Communication.Api/
COPY communication.service/src/Communication.Domain/Communication.Domain.csproj communication.service/src/Communication.Domain/
COPY communication.service/src/Communication.Application/Communication.Application.csproj communication.service/src/Communication.Application/
COPY communication.service/src/Communication.Infrastructure/Communication.Infrastructure.csproj communication.service/src/Communication.Infrastructure/
COPY gn-core-libraries/GnDapper/GnDapper.csproj gn-core-libraries/GnDapper/
COPY gn-core-libraries/GnSecurity/GnSecurity.csproj gn-core-libraries/GnSecurity/
COPY gn-core-libraries/GnMessaging/GnMessaging.csproj gn-core-libraries/GnMessaging/
COPY gn-core-libraries/GnValidation/GnValidation.csproj gn-core-libraries/GnValidation/
RUN dotnet restore communication.service/src/Communication.Api/Communication.Api.csproj
COPY . .
RUN dotnet build communication.service/src/Communication.Api/Communication.Api.csproj -c Release -o /app/build
FROM build AS publish
RUN dotnet publish communication.service/src/Communication.Api/Communication.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5007
ENV ASPNETCORE_URLS=http://+:5007
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Communication.Api.dll"]
