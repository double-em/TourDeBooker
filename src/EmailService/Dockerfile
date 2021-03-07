FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
RUN apt-get update
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Core/ Core/
COPY EmailService/EmailService.csproj EmailService/
RUN dotnet restore EmailService/EmailService.csproj

COPY EmailService/ EmailService/
RUN dotnet build EmailService/EmailService.csproj -c Release -o /out/build

FROM build AS publish 
RUN dotnet publish EmailService/EmailService.csproj -c Release -o /out/publish

FROM base AS final
COPY --from=publish /out/publish .
ENTRYPOINT ["dotnet", "EmailService.dll"]