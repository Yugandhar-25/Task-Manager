# ── Stage 1: Build ──────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY ["TaskManager.API/Task Manager API.csproj", "TaskManager.API/"]
RUN dotnet restore "TaskManager.API/Task Manager API.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/TaskManager.API"
RUN dotnet publish "Task Manager API.csproj" -c Release -o /app/publish

# ── Stage 2: Runtime ─────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Task Manager API.dll"]