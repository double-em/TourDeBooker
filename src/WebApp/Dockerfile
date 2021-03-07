FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
RUN apt-get update
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY WebApp/WebApp.csproj WebApp/
COPY WebApp/nuget.config .
COPY ["*.props", "/"]

RUN dotnet restore WebApp/WebApp.csproj --configfile "./nuget.config"

COPY . .
RUN dotnet build WebApp/WebApp.csproj -c Release -o /out/build --no-restore

FROM build AS publish 
RUN dotnet publish WebApp/WebApp.csproj -c Release -o /out/publish --no-restore

FROM base AS final
COPY --from=publish /out/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]