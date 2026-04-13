# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copy everything and restore
COPY . ./
RUN dotnet restore

# Build and publish
RUN dotnet publish StripePoc.Api/StripePoc.Api.csproj -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose port and start
EXPOSE 80
ENTRYPOINT ["dotnet", "StripePoc.Api.dll"]
