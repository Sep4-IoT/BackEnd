# Use the official ASP.NET Core runtime image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official ASP.NET Core SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["BackEndAPI/BackEndAPI.csproj", "BackEndAPI/"]
COPY ["Domain/Domain.csproj", "Domain/"]

RUN dotnet restore "BackEndAPI/BackEndAPI.csproj"

# Copy the remaining source code and build the project
COPY . .

WORKDIR "/src/BackEndAPI"
RUN dotnet build "BackEndAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "BackEndAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official ASP.NET Core runtime image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackEndAPI.dll"]
