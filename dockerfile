# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["sarw-rp.csproj", "./"]
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Generate dev certificate during build
RUN mkdir -p /https
RUN dotnet dev-certs https -ep /https/aspnetapp.pfx -p YourSecurePassword

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published app
COPY --from=build /app/publish .

# Copy the generated certificate
COPY --from=build /https /https

# Configure the container
EXPOSE 7001
EXPOSE 5295

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5295;https://+:7001
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=YourSecurePassword

ENTRYPOINT ["dotnet", "sarw-rp.dll"]