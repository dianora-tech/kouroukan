FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY support.service/src/Support.Api/Support.Api.csproj support.service/src/Support.Api/
COPY support.service/src/Support.Domain/Support.Domain.csproj support.service/src/Support.Domain/
COPY support.service/src/Support.Application/Support.Application.csproj support.service/src/Support.Application/
COPY support.service/src/Support.Infrastructure/Support.Infrastructure.csproj support.service/src/Support.Infrastructure/
COPY gn-core-libraries/GnDapper/GnDapper.csproj gn-core-libraries/GnDapper/
COPY gn-core-libraries/GnSecurity/GnSecurity.csproj gn-core-libraries/GnSecurity/
COPY gn-core-libraries/GnMessaging/GnMessaging.csproj gn-core-libraries/GnMessaging/
COPY gn-core-libraries/GnValidation/GnValidation.csproj gn-core-libraries/GnValidation/

RUN dotnet restore support.service/src/Support.Api/Support.Api.csproj

COPY . .

RUN dotnet build support.service/src/Support.Api/Support.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish support.service/src/Support.Api/Support.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5011
ENV ASPNETCORE_URLS=http://+:5011
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Support.Api.dll"]
