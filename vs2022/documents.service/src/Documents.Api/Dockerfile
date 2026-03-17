FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY documents.service/src/Documents.Api/Documents.Api.csproj documents.service/src/Documents.Api/
COPY documents.service/src/Documents.Domain/Documents.Domain.csproj documents.service/src/Documents.Domain/
COPY documents.service/src/Documents.Application/Documents.Application.csproj documents.service/src/Documents.Application/
COPY documents.service/src/Documents.Infrastructure/Documents.Infrastructure.csproj documents.service/src/Documents.Infrastructure/
COPY gn-core-libraries/GnDapper/GnDapper.csproj gn-core-libraries/GnDapper/
COPY gn-core-libraries/GnSecurity/GnSecurity.csproj gn-core-libraries/GnSecurity/
COPY gn-core-libraries/GnMessaging/GnMessaging.csproj gn-core-libraries/GnMessaging/
COPY gn-core-libraries/GnValidation/GnValidation.csproj gn-core-libraries/GnValidation/
RUN dotnet restore documents.service/src/Documents.Api/Documents.Api.csproj
COPY . .
RUN dotnet build documents.service/src/Documents.Api/Documents.Api.csproj -c Release -o /app/build
FROM build AS publish
RUN dotnet publish documents.service/src/Documents.Api/Documents.Api.csproj -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5009
ENV ASPNETCORE_URLS=http://+:5009
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Documents.Api.dll"]
