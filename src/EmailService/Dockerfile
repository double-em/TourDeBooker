FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
RUN apt-get update
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY EmailService/EmailService.csproj EmailService/
COPY EmailService/nuget.config .
COPY ["*.props", "/"]

RUN dotnet restore EmailService/EmailService.csproj --configfile "./nuget.config"

COPY . .
RUN dotnet build EmailService/EmailService.csproj -c Release -o /out/build --no-restore

FROM build AS publish 
RUN dotnet publish EmailService/EmailService.csproj -c Release -o /out/publish --no-restore

FROM base AS final
COPY --from=publish /out/publish .
ENTRYPOINT ["dotnet", "EmailService.dll"]