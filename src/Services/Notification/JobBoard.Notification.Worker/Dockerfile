FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Services/Notification/JobBoard.Notification.Worker/JobBoard.Notification.Worker.csproj", "src/Services/Notification/JobBoard.Notification.Worker/"]
COPY ["src/Shared/JobBoard.Shared/JobBoard.Shared.csproj", "src/Shared/JobBoard.Shared/"]

RUN dotnet restore "src/Services/Notification/JobBoard.Notification.Worker/JobBoard.Notification.Worker.csproj"

COPY . .
WORKDIR "/src/src/Services/Notification/JobBoard.Notification.Worker"
RUN dotnet build "./JobBoard.Notification.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JobBoard.Notification.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobBoard.Notification.Worker.dll"]