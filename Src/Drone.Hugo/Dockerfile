FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Src/Drone.Hugo/Drone.Hugo.csproj", "Drone.Hugo/"]
RUN dotnet restore "Drone.HugoTP/Drone.Hugo.csproj"
COPY . .
WORKDIR "/src/Src/Drone.Hugo"
RUN dotnet build "Drone.Hugo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Drone.Hugo.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=publish /app/publish /app
ENTRYPOINT ["dotnet", "/app/Drone.Hugo.dll"]