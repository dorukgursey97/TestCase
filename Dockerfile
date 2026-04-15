# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/AppTemplate.Web/AppTemplate.Web.csproj",             "src/AppTemplate.Web/"]
COPY ["src/AppTemplate.Application/AppTemplate.Application.csproj", "src/AppTemplate.Application/"]
COPY ["src/AppTemplate.Infrastructure/AppTemplate.Infrastructure.csproj", "src/AppTemplate.Infrastructure/"]
COPY ["src/AppTemplate.Domain/AppTemplate.Domain.csproj",       "src/AppTemplate.Domain/"]

RUN dotnet restore "src/AppTemplate.Web/AppTemplate.Web.csproj"

COPY . .
WORKDIR "/src/src/AppTemplate.Web"
RUN dotnet publish "AppTemplate.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AppTemplate.Web.dll"]
