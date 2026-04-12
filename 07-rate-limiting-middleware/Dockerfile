FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/RateLimitingMiddleware.Api/RateLimitingMiddleware.Api.csproj src/RateLimitingMiddleware.Api/
RUN dotnet restore src/RateLimitingMiddleware.Api/RateLimitingMiddleware.Api.csproj

COPY src/ src/
RUN dotnet publish src/RateLimitingMiddleware.Api/RateLimitingMiddleware.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RateLimitingMiddleware.Api.dll"]
