# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["MonkeyShelter.sln", "."]
COPY ["src/MonkeyShelter.Application/MonkeyShelter.Application.csproj", "src/MonkeyShelter.Application/"]
COPY ["src/MonkeyShelter.Infrastructure/MonkeyShelter.Infrastructure.csproj", "src/MonkeyShelter.Infrastructure/"]
COPY ["src/MonkeyShelter.Auth/MonkeyShelter.Auth.csproj", "src/MonkeyShelter.Auth/"]

# Restore only Auth (pulls in the other two via project references)
RUN dotnet restore "src/MonkeyShelter.Auth/MonkeyShelter.Auth.csproj"

# Copy the rest of the code
COPY . .

# Publish Auth into /app/publish without an AppHost
WORKDIR "/src/src/MonkeyShelter.Auth"
RUN dotnet publish "MonkeyShelter.Auth.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose the Auth port
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5270
EXPOSE 5270

ENTRYPOINT ["dotnet", "MonkeyShelter.Auth.dll"]
