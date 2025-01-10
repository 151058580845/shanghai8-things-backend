#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/HgznMes.WebApi/HgznMes.WebApi.csproj", "src/HgznMes.WebApi/"]
COPY ["src/HgznMes.Application/HgznMes.Application.csproj", "src/HgznMes.Application/"]
COPY ["src/HgznMes.Domain.Shared/HgznMes.Domain.Shared.csproj", "src/HgznMes.Domain.Shared/"]
COPY ["src/HgznMes.Domain/HgznMes.Domain.csproj", "src/HgznMes.Domain/"]
COPY ["src/HgznMes.Infrastructure/HgznMes.Infrastructure.csproj", "src/HgznMes.Infrastructure/"]
RUN dotnet restore "./src/HgznMes.WebApi/HgznMes.WebApi.csproj"
COPY . .
WORKDIR "/src/src/HgznMes.WebApi"
RUN dotnet build "./HgznMes.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HgznMes.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HgznMes.WebApi.dll"]