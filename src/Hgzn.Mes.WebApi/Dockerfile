#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER root

# Alpine 使用 apk 包管理器，安装 ffmpeg
RUN apk update && \
    apk add --no-cache ffmpeg

# 验证安装
RUN ffmpeg -version

USER app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN dotnet nuget remove source "nuget.org" && dotnet nuget add source "https://mirrors.huaweicloud.com/repository/nuget/v3/index.json" --name "cn" && dotnet nuget enable source "cn"
COPY ["src/Hgzn.Mes.WebApi/Hgzn.Mes.WebApi.csproj", "src/Hgzn.Mes.WebApi/"]
COPY ["src/Hgzn.Mes.Application.Main/Hgzn.Mes.Application.Main.csproj", "src/Hgzn.Mes.Application.Main/"]
COPY ["src/Hgzn.Mes.Infrastructure/Hgzn.Mes.Infrastructure.csproj", "src/Hgzn.Mes.Infrastructure/"]
COPY ["src/Hgzn.Mes.Domain/Hgzn.Mes.Domain.csproj", "src/Hgzn.Mes.Domain/"]
COPY ["src/Hgzn.Mes.Domain.Shared/Hgzn.Mes.Domain.Shared.csproj", "src/Hgzn.Mes.Domain.Shared/"]
RUN dotnet restore "./src/Hgzn.Mes.WebApi/Hgzn.Mes.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Hgzn.Mes.WebApi"
RUN dotnet build "./Hgzn.Mes.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Hgzn.Mes.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hgzn.Mes.WebApi.dll"]