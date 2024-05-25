# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["SE172788.ProductManagement.API/SE172788.ProductManagement.API.csproj", "SE172788.ProductManagement.API/"]
COPY ["SE172788.ProductManagement.Repo/SE172788.ProductManagement.Repo.csproj", "SE172788.ProductManagement.Repo/"]
RUN dotnet restore "./SE172788.ProductManagement.API/SE172788.ProductManagement.API.csproj"

# Copy the rest of the source code and build the application
COPY . .
WORKDIR "/src/SE172788.ProductManagement.API"
RUN dotnet build "./SE172788.ProductManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SE172788.ProductManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SE172788.ProductManagement.API.dll"]
