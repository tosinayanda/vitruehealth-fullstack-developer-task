# ------------------------------------
# 1. BUILD STAGE
# Use the .NET 8 SDK on the Alpine variant for building
# ------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy the .csproj file(s) and restore dependencies
# This step is often cached, speeding up subsequent builds if only source files change
COPY *.csproj .
RUN dotnet restore

# Copy the remaining source code
COPY . .

# Publish the application to the 'app/publish' directory
RUN dotnet publish -c Release -o /app/publish --no-restore

# ------------------------------------
# 2. FINAL STAGE (RUNTIME)
# Use the .NET 8 ASP.NET runtime on the Alpine variant
# ------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# The standard ASP.NET Core port
ENV ASPNETCORE_URLS=http://+:8080

# Expose the port (informative only, doesn't publish the port)
EXPOSE 8080

# Define the entry point to run the application
ENTRYPOINT ["dotnet", "Suggestions.API.dll"]