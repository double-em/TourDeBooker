FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
RUN apt-get update
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Core/ Core/
COPY WebApp/WebApp.csproj WebApp/
RUN dotnet restore WebApp/WebApp.csproj

COPY WebApp/ WebApp/
RUN dotnet build WebApp/WebApp.csproj -c Release -o /out/build

FROM build AS publish 
RUN dotnet publish WebApp/WebApp.csproj -c Release -o /out/publish

FROM base AS final
COPY --from=publish /out/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]