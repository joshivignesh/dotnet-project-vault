FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/SignalRDashboard.Api/SignalRDashboard.Api.csproj", "src/SignalRDashboard.Api/"]
RUN dotnet restore "src/SignalRDashboard.Api/SignalRDashboard.Api.csproj"
COPY . .
RUN dotnet publish "src/SignalRDashboard.Api/SignalRDashboard.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SignalRDashboard.Api.dll"]
