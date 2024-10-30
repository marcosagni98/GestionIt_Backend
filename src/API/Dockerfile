# Usar la imagen base .NET runtime con Ubuntu
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Usar la imagen .NET SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/API/API.csproj", "src/API/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infraestructure/Infraestructure.csproj", "src/Infraestructure/"]
RUN dotnet restore "./src/API/API.csproj"
COPY . .
WORKDIR "/src/src/API"
RUN dotnet build "./API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar la aplicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final para ejecutar la aplicación
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Instalar paquetes requeridos para SQL Server
RUN apt-get update && \
    apt-get install -y curl apt-transport-https gnupg && \
    rm -rf /var/lib/apt/lists/*

# Punto de entrada
ENTRYPOINT ["dotnet", "API.dll"]
