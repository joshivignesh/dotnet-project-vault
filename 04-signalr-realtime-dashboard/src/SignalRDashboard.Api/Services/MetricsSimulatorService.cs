using Microsoft.AspNetCore.SignalR;
using SignalRDashboard.Api.Hubs;
using SignalRDashboard.Api.Models;

namespace SignalRDashboard.Api.Services;

/// <summary>
/// Background service that simulates server metrics and broadcasts them
/// to all connected SignalR dashboard clients every 2 seconds.
/// In production, replace the simulator with real process/infrastructure telemetry.
/// </summary>
public sealed class MetricsSimulatorService(
    IHubContext<DashboardHub> hub,
    ILogger<MetricsSimulatorService> logger) : BackgroundService
{
    private readonly Random _rng = new();

    // Simulated "baseline" state that trends over time
    private double _cpuBase = 35.0;
    private double _memBase = 52.0;
    private long _rpsBase = 180;
    private int _connectionsBase = 24;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Metrics simulator started — broadcasting every 2s");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var metrics = GenerateMetrics();
                await hub.Clients.All.SendAsync("ReceiveMetrics", metrics, stoppingToken);
                logger.LogDebug("Broadcast: CPU={Cpu:F1}% MEM={Mem:F1}% RPS={Rps}", 
                    metrics.CpuPercent, metrics.MemoryPercent, metrics.RequestsPerSecond);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error broadcasting metrics");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }

        logger.LogInformation("Metrics simulator stopped");
    }

    private SystemMetrics GenerateMetrics()
    {
        // Slowly drift baselines to simulate realistic load patterns
        _cpuBase = Math.Clamp(_cpuBase + _rng.NextDouble() * 4 - 2, 5, 95);
        _memBase = Math.Clamp(_memBase + _rng.NextDouble() * 2 - 1, 20, 90);
        _rpsBase = Math.Clamp(_rpsBase + (long)(_rng.NextDouble() * 20 - 10), 10, 1000);
        _connectionsBase = Math.Clamp(_connectionsBase + _rng.Next(-3, 4), 0, 500);

        // Add instantaneous jitter
        var cpu = Math.Clamp(_cpuBase + (_rng.NextDouble() * 10 - 5), 0, 100);
        var mem = Math.Clamp(_memBase + (_rng.NextDouble() * 4 - 2), 0, 100);
        var rps = Math.Max(0, _rpsBase + (long)(_rng.NextDouble() * 30 - 15));
        var avgResponseMs = Math.Max(0.5, 12.0 + cpu * 0.4 + _rng.NextDouble() * 8);
        var connections = Math.Max(0, _connectionsBase + _rng.Next(-5, 6));
        var errorRate = Math.Max(0, (cpu > 80 ? 0.05 : 0.001) + _rng.NextDouble() * 0.01);

        return new SystemMetrics(
            CpuPercent: Math.Round(cpu, 1),
            MemoryPercent: Math.Round(mem, 1),
            RequestsPerSecond: rps,
            AvgResponseMs: Math.Round(avgResponseMs, 1),
            ActiveConnections: connections,
            ErrorRate: Math.Round(errorRate, 4),
            Timestamp: DateTime.UtcNow
        );
    }
}
