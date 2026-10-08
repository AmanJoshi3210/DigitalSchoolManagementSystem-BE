FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first so `dotnet restore` is cached until a csproj changes.
COPY DigitalSchoolManagementSystem.API/*.csproj DigitalSchoolManagementSystem.API/
COPY DigitalSchoolManagementSystem.Application/*.csproj DigitalSchoolManagementSystem.Application/
COPY DigitalSchoolManagementSystem.Domain/*.csproj DigitalSchoolManagementSystem.Domain/
COPY DigitalSchoolManagementSystem.Infrastructure/*.csproj DigitalSchoolManagementSystem.Infrastructure/
COPY DigitalSchoolManagementSystem.Shared/*.csproj DigitalSchoolManagementSystem.Shared/
RUN dotnet restore DigitalSchoolManagementSystem.API/DigitalSchoolManagementSystem.API.csproj

COPY . .
RUN dotnet publish DigitalSchoolManagementSystem.API/DigitalSchoolManagementSystem.API.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "DigitalSchoolManagementSystem.API.dll"]
