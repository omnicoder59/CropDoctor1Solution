# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["CropDoctor1/CropDoctor1.csproj", "CropDoctor1/"]
RUN dotnet restore "CropDoctor1/CropDoctor1.csproj"

# Copy source code
COPY . .
WORKDIR "/src/CropDoctor1"

# Build the application
RUN dotnet build "CropDoctor1.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CropDoctor1.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app from publish stage
COPY --from=publish /app/publish .

# Expose port
EXPOSE 80
EXPOSE 443

# Set environment variables (can be overridden in docker-compose or docker run)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost/ || exit 1

ENTRYPOINT ["dotnet", "CropDoctor1.dll"]
