# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/WorkShop.API/WorkShop.API.csproj", "src/WorkShop.API/"]
COPY ["src/WorkShop.Application/WorkShop.Application.csproj", "src/WorkShop.Application/"]
COPY ["src/WorkShop.Domain/WorkShop.Domain.csproj", "src/WorkShop.Domain/"]
COPY ["src/WorkShop.Infrastructure/WorkShop.Infrastructure.csproj", "src/WorkShop.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/WorkShop.API/WorkShop.API.csproj"

# Copy all source files
COPY . .

# Build and publish the application
WORKDIR "/src/src/WorkShop.API"
RUN dotnet build "WorkShop.API.csproj" -c Release -o /app/build
RUN dotnet publish "WorkShop.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published application
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "WorkShop.API.dll"]
