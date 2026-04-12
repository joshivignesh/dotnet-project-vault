FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/KedaWorker.Worker/KedaWorker.Worker.csproj", "src/KedaWorker.Worker/"]
RUN dotnet restore "src/KedaWorker.Worker/KedaWorker.Worker.csproj"
COPY . .
RUN dotnet publish "src/KedaWorker.Worker/KedaWorker.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KedaWorker.Worker.dll"]
