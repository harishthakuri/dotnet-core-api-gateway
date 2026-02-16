# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file
COPY ["tests/MockServices/MockUserService/MockUserService.csproj", "tests/MockServices/MockUserService/"]

# Restore dependencies
RUN dotnet restore "tests/MockServices/MockUserService/MockUserService.csproj"

# Copy source code
COPY tests/MockServices/MockUserService/ tests/MockServices/MockUserService/

# Build application
WORKDIR "/src/tests/MockServices/MockUserService"
RUN dotnet build "MockUserService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "MockUserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

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
    SERVICE_NAME=user-service \
    SERVICE_PORT=8080

# Entry point
ENTRYPOINT ["dotnet", "MockUserService.dll"]
