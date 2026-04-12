FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/MinimalApiJwtOpenApi.Api/MinimalApiJwtOpenApi.Api.csproj", "src/MinimalApiJwtOpenApi.Api/"]
RUN dotnet restore "src/MinimalApiJwtOpenApi.Api/MinimalApiJwtOpenApi.Api.csproj"
COPY . .
RUN dotnet publish "src/MinimalApiJwtOpenApi.Api/MinimalApiJwtOpenApi.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinimalApiJwtOpenApi.Api.dll"]
