# IOTController/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 50000
EXPOSE 6000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IOTController/IOTController.csproj", "IOTController/"]
RUN dotnet restore "IOTController/IOTController.csproj"
COPY . .
WORKDIR "/src/IOTController"
RUN dotnet build "IOTController.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IOTController.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IOTController.dll"]
