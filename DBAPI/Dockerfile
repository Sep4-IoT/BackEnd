# Use the official ASP.NET Core runtime image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official ASP.NET Core SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the DBAPI and Domain project files
COPY ["DBAPI/DBAPI.csproj", "DBAPI/"]
COPY ["Domain/Domain.csproj", "Domain/"]

# Restore dependencies for both projects
RUN dotnet restore "DBAPI/DBAPI.csproj"

# Copy the remaining source code
COPY . .

# Set the working directory to the DBAPI project
WORKDIR "/src/DBAPI"

# Build the DBAPI project
RUN dotnet build "DBAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "DBAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official ASP.NET Core runtime image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DBAPI.dll"]
