FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY pedagogie.service/src/Pedagogie.Api/Pedagogie.Api.csproj pedagogie.service/src/Pedagogie.Api/
COPY pedagogie.service/src/Pedagogie.Domain/Pedagogie.Domain.csproj pedagogie.service/src/Pedagogie.Domain/
COPY pedagogie.service/src/Pedagogie.Application/Pedagogie.Application.csproj pedagogie.service/src/Pedagogie.Application/
COPY pedagogie.service/src/Pedagogie.Infrastructure/Pedagogie.Infrastructure.csproj pedagogie.service/src/Pedagogie.Infrastructure/
COPY gn-core-libraries/GnDapper/GnDapper.csproj gn-core-libraries/GnDapper/
COPY gn-core-libraries/GnSecurity/GnSecurity.csproj gn-core-libraries/GnSecurity/
COPY gn-core-libraries/GnMessaging/GnMessaging.csproj gn-core-libraries/GnMessaging/
COPY gn-core-libraries/GnValidation/GnValidation.csproj gn-core-libraries/GnValidation/
RUN dotnet restore pedagogie.service/src/Pedagogie.Api/Pedagogie.Api.csproj
COPY . .
RUN dotnet build pedagogie.service/src/Pedagogie.Api/Pedagogie.Api.csproj -c Release -o /app/build
FROM build AS publish
RUN dotnet publish pedagogie.service/src/Pedagogie.Api/Pedagogie.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5002
ENV ASPNETCORE_URLS=http://+:5002
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Pedagogie.Api.dll"]
