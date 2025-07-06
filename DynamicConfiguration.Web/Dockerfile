FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DynamicConfiguration.Web/DynamicConfiguration.Web.csproj", "DynamicConfiguration.Web/"]
RUN dotnet restore "DynamicConfiguration.Web/DynamicConfiguration.Web.csproj"
COPY . .
WORKDIR "/src/DynamicConfiguration.Web"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DynamicConfiguration.Web.dll"]
