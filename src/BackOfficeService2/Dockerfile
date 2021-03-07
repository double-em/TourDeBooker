FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
RUN apt-get update
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY Core/ Core/
COPY BackOfficeService/BackOfficeService.csproj BackOfficeService/
RUN dotnet restore BackOfficeService/BackOfficeService.csproj

COPY BackOfficeService/ BackOfficeService/
RUN dotnet build BackOfficeService/BackOfficeService.csproj -c Release -o /out/build

FROM build AS publish 
RUN dotnet publish BackOfficeService/BackOfficeService.csproj -c Release -o /out/publish

FROM base AS final
COPY --from=publish /out/publish .
ENTRYPOINT ["dotnet", "BackOfficeService.dll"]