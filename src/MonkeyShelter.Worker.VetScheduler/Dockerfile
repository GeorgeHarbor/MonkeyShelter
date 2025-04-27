# Dockerfile for MonkeyShelter.Worker.VetScheduler

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution file
COPY ["MonkeyShelter.sln", "."]

# Copy csproj files for the Worker and its dependencies
COPY ["src/MonkeyShelter.Worker.VetScheduler/MonkeyShelter.Worker.VetScheduler.csproj", "src/MonkeyShelter.Worker.VetScheduler/"]
COPY ["src/MonkeyShelter.Application/MonkeyShelter.Application.csproj", "src/MonkeyShelter.Application/"]
COPY ["src/MonkeyShelter.Infrastructure/MonkeyShelter.Infrastructure.csproj", "src/MonkeyShelter.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/MonkeyShelter.Worker.VetScheduler/MonkeyShelter.Worker.VetScheduler.csproj"

# Copy the remaining source code
COPY ["src/", "src/"]

# Publish the worker
RUN dotnet publish "src/MonkeyShelter.Worker.VetScheduler/MonkeyShelter.Worker.VetScheduler.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published output from build stage
COPY --from=build /app/publish .

# Environment (if needed)
ENV ASPNETCORE_ENVIRONMENT=Production

# No ports to expose for a background worker

ENTRYPOINT ["dotnet", "MonkeyShelter.Worker.VetScheduler.dll"]
