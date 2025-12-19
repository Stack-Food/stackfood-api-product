# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY StackFood.Products.sln ./
COPY src/StackFood.Products.Domain/StackFood.Products.Domain.csproj src/StackFood.Products.Domain/
COPY src/StackFood.Products.Application/StackFood.Products.Application.csproj src/StackFood.Products.Application/
COPY src/StackFood.Products.Infrastructure/StackFood.Products.Infrastructure.csproj src/StackFood.Products.Infrastructure/
COPY src/StackFood.Products.API/StackFood.Products.API.csproj src/StackFood.Products.API/
COPY tests/StackFood.Products.Tests/StackFood.Products.Tests.csproj tests/StackFood.Products.Tests/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY src/ src/
COPY tests/ tests/

# Build the application
WORKDIR /src/src/StackFood.Products.API
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StackFood.Products.API.dll"]
