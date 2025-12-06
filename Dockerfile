# Use the official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution file
COPY *.sln ./

# Copy all project files
COPY SmartParkingSystem/*.csproj ./SmartParkingSystem/
COPY SmartParking.Core/*.csproj ./SmartParking.Core/
COPY SmartParking.Repository/*.csproj ./SmartParking.Repository/

# Restore dependencies
RUN dotnet restore

# Copy all remaining source code
COPY . ./

# Build the main project
WORKDIR /app/SmartParkingSystem
RUN dotnet publish -c Release -o out

# Use runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/SmartParkingSystem/out ./

# Expose the port your app uses
EXPOSE 5000

# Start the app
ENTRYPOINT ["dotnet", "SmartParkingSystem.dll"]

