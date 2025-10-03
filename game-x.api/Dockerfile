# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["game-x.api/game-x.api.csproj", "game-x.api/"]
COPY ["game-x.application/game-x.application.csproj", "game-x.application/"]
COPY ["game-x.domain/game-x.domain.csproj", "game-x.domain/"]
COPY ["game-x.infrastructure/game-x.infrastructure.csproj", "game-x.infrastructure/"]
COPY ["game-x.persistence/game-x.persistence.csproj", "game-x.persistence/"]
COPY ["game-x.share/game-x.share.csproj", "game-x.share/"]
RUN dotnet restore "./game-x.api/game-x.api.csproj"
COPY . .
WORKDIR "/src/game-x.api"
RUN dotnet build "./game-x.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./game-x.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "game-x.api.dll"]