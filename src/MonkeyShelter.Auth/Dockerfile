FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY MonkeyShelter.Auth.csproj ./
RUN dotnet restore "MonkeyShelter.Auth.csproj"

# Copy source code and publish
COPY . ./
RUN dotnet publish "MonkeyShelter.Auth.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Set environment and URLs from launchSettings
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS="http://+:5270"

# Expose ports for HTTP and HTTPS
EXPOSE 5270

ENTRYPOINT ["dotnet", "MonkeyShelter.Auth.dll"]
