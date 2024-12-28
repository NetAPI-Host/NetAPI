# Use Microsoft's official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy project files and restore dependencies
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out

# Use Microsoft's official ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the port Render will use
EXPOSE 5000

# Command to run your application
ENTRYPOINT ["dotnet", "NetApi.dll"]
