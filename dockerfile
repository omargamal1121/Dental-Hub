# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

# copy csproj files
COPY DentalHub.Domain/*.csproj DentalHub.Domain/
COPY DentalHub.Infrastructure/*.csproj DentalHub.Infrastructure/
COPY DentalHub.Application/*.csproj DentalHub.Application/
COPY DentalHub.API/*.csproj DentalHub.API/

RUN dotnet restore

# copy full source
COPY . .

WORKDIR /app/DentalHub.API
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app/DentalHub.API/out .

ENTRYPOINT ["dotnet", "DentalHub.API.dll"]