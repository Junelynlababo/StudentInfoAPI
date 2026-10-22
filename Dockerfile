# ==============================
# BUILD STAGE
# ==============================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy the actual project file
COPY ["StudentAPI.csproj", "./"]

# Restore dependencies
RUN dotnet restore "StudentAPI.csproj"

# Copy all application files
COPY . .

# Publish application
RUN dotnet publish "StudentAPI.csproj" -c Release -o /app/publish --no-restore


# ==============================
# RUNTIME STAGE
# ==============================

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# Render HTTP port
ENV ASPNETCORE_URLS=http://0.0.0.0:10000

EXPOSE 10000

# Start ASP.NET Core API
ENTRYPOINT ["dotnet", "StudentAPI.dll"]


-- COMMIT Name = First Database Query