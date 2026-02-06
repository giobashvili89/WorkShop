# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/WorkShop.API/WorkShop.API.csproj", "WorkShop.API/"]
COPY ["src/WorkShop.Application/WorkShop.Application.csproj", "WorkShop.Application/"]
COPY ["src/WorkShop.Domain/WorkShop.Domain.csproj", "WorkShop.Domain/"]
COPY ["src/WorkShop.Infrastructure/WorkShop.Infrastructure.csproj", "WorkShop.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "WorkShop.API/WorkShop.API.csproj"

# Copy all source files
COPY src/ .

# Build and publish
WORKDIR "/src/WorkShop.API"
RUN dotnet build "WorkShop.API.csproj" -c Release -o /app/build
RUN dotnet publish "WorkShop.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published files from build stage
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "WorkShop.API.dll"]
