FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5009
EXPOSE 5010

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["LocalNuget/", "/src/LocalNuget/"]
COPY ["nuget.config", "/src/"]


COPY ["RecommendationService/RecommendationService.API/RecommendationService.API.csproj", "RecommendationService.API/"]
COPY ["RecommendationService/RecommendationService.Application/RecommendationService.Application.csproj", "RecommendationService.Application/"]
COPY ["RecommendationService/RecommendationService.Domain/RecommendationService.Domain.csproj", "RecommendationService.Domain/"]
COPY ["RecommendationService/RecommendationService.Contracts/RecommendationService.Contracts.csproj", "RecommendationService.Contracts/"]
COPY ["RecommendationService/RecommendationService.Infrastructure/RecommendationService.Infrastructure.csproj", "RecommendationService.Infrastructure/"]
RUN dotnet restore "RecommendationService.API/RecommendationService.API.csproj"
COPY . .
WORKDIR "/src/RecommendationService/RecommendationService.API"
RUN dotnet build "./RecommendationService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RecommendationService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecommendationService.API.dll"]
