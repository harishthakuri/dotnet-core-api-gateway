# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/ApiGateway.Api/ApiGateway.Api.csproj", "src/ApiGateway.Api/"]

# Restore dependencies
RUN dotnet restore "src/ApiGateway.Api/ApiGateway.Api.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/src/ApiGateway.Api"
RUN dotnet build "ApiGateway.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ApiGateway.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1000 appuser \
    && adduser --system --uid 1000 --ingroup appuser --shell /bin/sh appuser

# Copy published application
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "ApiGateway.Api.dll"]
