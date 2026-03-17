FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY evaluations.service/src/Evaluations.Api/Evaluations.Api.csproj evaluations.service/src/Evaluations.Api/
COPY evaluations.service/src/Evaluations.Domain/Evaluations.Domain.csproj evaluations.service/src/Evaluations.Domain/
COPY evaluations.service/src/Evaluations.Application/Evaluations.Application.csproj evaluations.service/src/Evaluations.Application/
COPY evaluations.service/src/Evaluations.Infrastructure/Evaluations.Infrastructure.csproj evaluations.service/src/Evaluations.Infrastructure/
COPY gn-core-libraries/GnDapper/GnDapper.csproj gn-core-libraries/GnDapper/
COPY gn-core-libraries/GnSecurity/GnSecurity.csproj gn-core-libraries/GnSecurity/
COPY gn-core-libraries/GnMessaging/GnMessaging.csproj gn-core-libraries/GnMessaging/
COPY gn-core-libraries/GnValidation/GnValidation.csproj gn-core-libraries/GnValidation/
RUN dotnet restore evaluations.service/src/Evaluations.Api/Evaluations.Api.csproj
COPY . .
RUN dotnet build evaluations.service/src/Evaluations.Api/Evaluations.Api.csproj -c Release -o /app/build
FROM build AS publish
RUN dotnet publish evaluations.service/src/Evaluations.Api/Evaluations.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5003
ENV ASPNETCORE_URLS=http://+:5003
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Evaluations.Api.dll"]
