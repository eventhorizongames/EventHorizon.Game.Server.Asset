FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY src/EventHorizon.Game.Server.Asset/EventHorizon.Game.Server.Asset.csproj src/EventHorizon.Game.Server.Asset/
COPY src/EventHorizon.BackgroundTasks/EventHorizon.BackgroundTasks.csproj src/EventHorizon.BackgroundTasks/
COPY src/EventHorizon.Platform.Integration/EventHorizon.Platform.Integration.csproj src/EventHorizon.Platform.Integration/

RUN dotnet restore "src/EventHorizon.Game.Server.Asset/EventHorizon.Game.Server.Asset.csproj"
COPY . .
RUN dotnet build --no-restore src/EventHorizon.Game.Server.Asset/EventHorizon.Game.Server.Asset.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /source
RUN dotnet publish --no-restore src/EventHorizon.Game.Server.Asset/EventHorizon.Game.Server.Asset.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventHorizon.Game.Server.Asset.dll"]
