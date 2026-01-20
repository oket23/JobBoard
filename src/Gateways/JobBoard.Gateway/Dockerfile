FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Gateways/JobBoard.Gateway/JobBoard.Gateway.csproj", "src/Gateways/JobBoard.Gateway/"]
RUN dotnet restore "src/Gateways/JobBoard.Gateway/JobBoard.Gateway.csproj"
COPY . .
WORKDIR "/src/src/Gateways/JobBoard.Gateway"
RUN dotnet build "./JobBoard.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JobBoard.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

USER root

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

USER $APP_UID

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobBoard.Gateway.dll"]
