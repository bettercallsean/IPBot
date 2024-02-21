#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["IPBot.API/IPBot.API.csproj", "IPBot.API/"]
COPY ["IPBot.API.Domain/IPBot.API.Domain.csproj", "IPBot.API.Domain/"]
RUN dotnet restore "./IPBot.API/./IPBot.API.csproj"
COPY . .
WORKDIR "/src/IPBot.API"
RUN dotnet build "./IPBot.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IPBot.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IPBot.API.dll"]

FROM python:3
RUN pip install --upgrade pip && \
	pip install python-a2s mcstatus python-dotenv