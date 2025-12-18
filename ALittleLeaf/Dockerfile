# Base image cho runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Stage build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ALittleLeaf/ALittleLeaf.csproj", "ALittleLeaf/"]

RUN dotnet restore "ALittleLeaf/ALittleLeaf.csproj"

COPY . .

WORKDIR "/src/ALittleLeaf"

RUN dotnet build "ALittleLeaf.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "ALittleLeaf.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ALittleLeaf.dll"]