# Multi-stage Docker build for the ASP.NET Core backend
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj first to leverage Docker layer caching for restore
COPY IauVacanta.Backend.csproj ./
RUN dotnet restore IauVacanta.Backend.csproj

# Copy source and publish
COPY . ./
RUN dotnet publish IauVacanta.Backend.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

# App listens on port 8080 inside the container
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "IauVacanta.Backend.dll"]
