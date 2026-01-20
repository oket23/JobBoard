FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Identity/JobBoard.Identity.Api/JobBoard.Identity.Api.csproj", "src/Services/Identity/JobBoard.Identity.Api/"]
COPY ["src/Services/Identity/JobBoard.Identity.Application/JobBoard.Identity.Application.csproj", "src/Services/Identity/JobBoard.Identity.Application/"]
COPY ["src/Services/Identity/JobBoard.Identity.Domain/JobBoard.Identity.Domain.csproj", "src/Services/Identity/JobBoard.Identity.Domain/"]
COPY ["src/Shared/JobBoard.Shared/JobBoard.Shared.csproj", "src/Shared/JobBoard.Shared/"]
COPY ["src/Services/Identity/JobBoard.Identity.Infrastructure/JobBoard.Identity.Infrastructure.csproj", "src/Services/Identity/JobBoard.Identity.Infrastructure/"]
RUN dotnet restore "src/Services/Identity/JobBoard.Identity.Api/JobBoard.Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Identity/JobBoard.Identity.Api"
RUN dotnet build "./JobBoard.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JobBoard.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobBoard.Identity.Api.dll"]