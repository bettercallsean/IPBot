FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["IPBot.API/IPBot.API.csproj", "IPBot.API/"]
COPY ["nuget.config", ""]
RUN dotnet restore "IPBot.API/IPBot.API.csproj"
COPY . .
WORKDIR "/src/IPBot.API"
RUN dotnet build "IPBot.API.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "IPBot.API.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IPBot.API.dll"]
