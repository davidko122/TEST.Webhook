# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copy csproj và restore
COPY ["TEST.Webhook/TEST.Webhook.csproj", "TEST.Webhook/"]
RUN dotnet restore "TEST.Webhook/TEST.Webhook.csproj"

# Copy toàn bộ source
COPY . .

# Build
WORKDIR "/src/TEST.Webhook"
RUN dotnet build "TEST.Webhook.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "TEST.Webhook.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Final runtime (nhẹ)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
USER app
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TEST.Webhook.dll"]