#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Client/Client.csproj", "Client/"]

RUN dotnet restore "./Client/Client.csproj"

COPY . .

WORKDIR "/src/Client"

RUN dotnet build "./Client.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM nginx:alpine AS final


# We'll copy all the contents from wwwroot in the publish
# folder into nginx/html for nginx to serve. The destination
# should be the same as what you set in the nginx.conf.
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .