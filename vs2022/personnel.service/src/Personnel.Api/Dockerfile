FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY personnel.service/src/Personnel.Api/Personnel.Api.csproj personnel.service/src/Personnel.Api/
COPY personnel.service/src/Personnel.Domain/Personnel.Domain.csproj personnel.service/src/Personnel.Domain/
COPY personnel.service/src/Personnel.Application/Personnel.Application.csproj personnel.service/src/Personnel.Application/
COPY personnel.service/src/Personnel.Infrastructure/Personnel.Infrastructure.csproj personnel.service/src/Personnel.Infrastructure/
COPY gn-core-libraries/GnDapper/GnDapper.csproj gn-core-libraries/GnDapper/
COPY gn-core-libraries/GnSecurity/GnSecurity.csproj gn-core-libraries/GnSecurity/
COPY gn-core-libraries/GnMessaging/GnMessaging.csproj gn-core-libraries/GnMessaging/
COPY gn-core-libraries/GnValidation/GnValidation.csproj gn-core-libraries/GnValidation/
RUN dotnet restore personnel.service/src/Personnel.Api/Personnel.Api.csproj
COPY . .
RUN dotnet build personnel.service/src/Personnel.Api/Personnel.Api.csproj -c Release -o /app/build
FROM build AS publish
RUN dotnet publish personnel.service/src/Personnel.Api/Personnel.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5006
ENV ASPNETCORE_URLS=http://+:5006
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Personnel.Api.dll"]
