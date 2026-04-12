namespace SignalRDashboard.Api.Models;

public record SystemMetrics(
    double CpuPercent,
    double MemoryPercent,
    long RequestsPerSecond,
    double AvgResponseMs,
    int ActiveConnections,
    double ErrorRate,
    DateTime Timestamp
);
