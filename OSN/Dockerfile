# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore OSN/OSN.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish OSN/OSN.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5069
ENTRYPOINT ["dotnet", "OSN.dll"]
