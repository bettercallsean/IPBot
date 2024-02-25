FROM mcr.microsoft.com/dotnet/runtime:8.0-jammy-chiseled AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["IPBot/IPBot.csproj", "IPBot/"]
COPY ["nuget.config", ""]
RUN dotnet restore "IPBot/IPBot.csproj"
COPY . .
WORKDIR "/src/IPBot"
RUN dotnet build "IPBot.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "IPBot.csproj" -c $configuration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IPBot.dll"]
