FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Recruitment/JobBoard.Recruitment.Api/JobBoard.Recruitment.Api.csproj", "src/Services/Recruitment/JobBoard.Recruitment.Api/"]
COPY ["src/Services/Recruitment/JobBoard.Recruitment.Application/JobBoard.Recruitment.Application.csproj", "src/Services/Recruitment/JobBoard.Recruitment.Application/"]
COPY ["src/Shared/JobBoard.Shared/JobBoard.Shared.csproj", "src/Shared/JobBoard.Shared/"]
COPY ["src/Services/Recruitment/JobBoard.Recruitment.Domain/JobBoard.Recruitment.Domain.csproj", "src/Services/Recruitment/JobBoard.Recruitment.Domain/"]
COPY ["src/Services/Recruitment/JobBoard.Recruitment.Infrastructure/JobBoard.Recruitment.Infrastructure.csproj", "src/Services/Recruitment/JobBoard.Recruitment.Infrastructure/"]
RUN dotnet restore "src/Services/Recruitment/JobBoard.Recruitment.Api/JobBoard.Recruitment.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Recruitment/JobBoard.Recruitment.Api"
RUN dotnet build "./JobBoard.Recruitment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JobBoard.Recruitment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobBoard.Recruitment.Api.dll"]