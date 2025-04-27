# stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1) copy only the Auth csproj and restore its NuGet packages
COPY MonkeyShelter.Auth.csproj ./
RUN dotnet restore "MonkeyShelter.Auth.csproj"

# 2) copy the rest of the Auth source and publish
COPY . ./
RUN dotnet publish "MonkeyShelter.Auth.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# copy published output
COPY --from=build /app/publish .

# environment & port
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS="http://+:5270"
EXPOSE 5270

ENTRYPOINT ["dotnet", "MonkeyShelter.Auth.dll"]
