FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DynamicConfiguration.TestServiceA-API/DynamicConfiguration.TestServiceA-API.csproj", "DynamicConfiguration.TestServiceA-API/"]
RUN dotnet restore "DynamicConfiguration.TestServiceA-API/DynamicConfiguration.TestServiceA-API.csproj"
COPY . .
WORKDIR "/src/DynamicConfiguration.TestServiceA-API"
RUN dotnet build "./DynamicConfiguration.TestServiceA-API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DynamicConfiguration.TestServiceA-API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DynamicConfiguration.TestServiceA-API.dll"]
