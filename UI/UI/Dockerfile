FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 6667


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UI/UI/UI.Server.csproj", "UI/UI/"]
COPY ["FastCommerce.Data/Data.csproj", "FastCommerce.Data/"]
COPY ["FastCommerce.Core/Core.csproj", "FastCommerce.Core/"]
COPY ["UI/UI.Client/UI.Client.csproj", "UI/UI.Client/"]
RUN dotnet restore "./UI/UI/UI.Server.csproj"
COPY . .
WORKDIR "/src/UI/UI"
RUN dotnet build "./UI.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UI.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UI.Server.dll"]