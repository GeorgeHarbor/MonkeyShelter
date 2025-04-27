# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["MonkeyShelter.sln", "."]
COPY ["src/MonkeyShelter.Audit/MonkeyShelter.Audit.csproj", "src/MonkeyShelter.Audit/"]

# Restore dependencies
RUN dotnet restore "src/MonkeyShelter.Audit/MonkeyShelter.Audit.csproj"

# Copy all source files and publish
COPY . .
WORKDIR /src/src/MonkeyShelter.Audit
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port and start
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "MonkeyShelter.Audit.dll"]
