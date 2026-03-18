# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY TodoApi.csproj ./
RUN dotnet restore

# Copy all files and build
COPY . .
RUN dotnet publish TodoApi.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 5021
EXPOSE 5021

ENTRYPOINT ["dotnet", "TodoApi.dll"]
