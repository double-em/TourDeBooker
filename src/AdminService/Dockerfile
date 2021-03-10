FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
RUN apt-get update
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY AdminService/AdminService.csproj AdminService/
COPY AdminService/nuget.config .
COPY ["*.props", "/"]

RUN dotnet restore AdminService/AdminService.csproj --configfile "./nuget.config"

COPY . .
RUN dotnet build AdminService/AdminService.csproj -c Release -o /out/build --no-restore

FROM build AS publish 
RUN dotnet publish AdminService/AdminService.csproj -c Release -o /out/publish --no-restore

FROM base AS final
COPY --from=publish /out/publish .
ENTRYPOINT ["dotnet", "AdminService.dll"]