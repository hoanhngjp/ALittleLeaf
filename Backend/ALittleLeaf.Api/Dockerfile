# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore
COPY ["Backend/ALittleLeaf.Api/ALittleLeaf.Api.csproj", "Backend/ALittleLeaf.Api/"]
RUN dotnet restore "Backend/ALittleLeaf.Api/ALittleLeaf.Api.csproj"

# Copy source and publish
COPY Backend/ALittleLeaf.Api/ Backend/ALittleLeaf.Api/
WORKDIR /src/Backend/ALittleLeaf.Api
RUN dotnet publish "ALittleLeaf.Api.csproj" -c Release -o /app/publish --no-restore

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ALittleLeaf.Api.dll"]
