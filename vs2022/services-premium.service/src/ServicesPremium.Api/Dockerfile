FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY services-premium.service/src/ServicesPremium.Api/ServicesPremium.Api.csproj services-premium.service/src/ServicesPremium.Api/
COPY services-premium.service/src/ServicesPremium.Domain/ServicesPremium.Domain.csproj services-premium.service/src/ServicesPremium.Domain/
COPY services-premium.service/src/ServicesPremium.Application/ServicesPremium.Application.csproj services-premium.service/src/ServicesPremium.Application/
COPY services-premium.service/src/ServicesPremium.Infrastructure/ServicesPremium.Infrastructure.csproj services-premium.service/src/ServicesPremium.Infrastructure/
COPY gn-core-libraries/GnDapper/GnDapper.csproj gn-core-libraries/GnDapper/
COPY gn-core-libraries/GnSecurity/GnSecurity.csproj gn-core-libraries/GnSecurity/
COPY gn-core-libraries/GnMessaging/GnMessaging.csproj gn-core-libraries/GnMessaging/
COPY gn-core-libraries/GnValidation/GnValidation.csproj gn-core-libraries/GnValidation/
RUN dotnet restore services-premium.service/src/ServicesPremium.Api/ServicesPremium.Api.csproj
COPY . .
RUN dotnet build services-premium.service/src/ServicesPremium.Api/ServicesPremium.Api.csproj -c Release -o /app/build
FROM build AS publish
RUN dotnet publish services-premium.service/src/ServicesPremium.Api/ServicesPremium.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5010
ENV ASPNETCORE_URLS=http://+:5010
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "ServicesPremium.Api.dll"]
