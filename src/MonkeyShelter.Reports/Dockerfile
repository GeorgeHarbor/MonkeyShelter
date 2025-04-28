# stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1) copy the solution (so project-to-project refs work)
COPY ["MonkeyShelter.sln", "."]

# 2) copy each csproj for restore
COPY ["src/MonkeyShelter.Reports/MonkeyShelter.Reports.csproj",           "src/MonkeyShelter.Reports/"]
COPY ["src/MonkeyShelter.Application/MonkeyShelter.Application.csproj", "src/MonkeyShelter.Application/"]
COPY ["src/MonkeyShelter.Infrastructure/MonkeyShelter.Infrastructure.csproj", "src/MonkeyShelter.Infrastructure/"]

# 3) restore dependencies (API pull in Application & Infrastructure)
RUN dotnet restore "src/MonkeyShelter.Reports/MonkeyShelter.Reports.csproj"

# 4) copy all source files
COPY ["src/", "src/"]

# 5) publish
RUN dotnet publish "src/MonkeyShelter.Reports/MonkeyShelter.Reports.csproj" \
    -c Release -o /app/publish /p:UseAppHost=false

# stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# copy over published output
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS="http://+:5109"
EXPOSE 5109

ENTRYPOINT ["dotnet", "MonkeyShelter.Reports.dll"]
