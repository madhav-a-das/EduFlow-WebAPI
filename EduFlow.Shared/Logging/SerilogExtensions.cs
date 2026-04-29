using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace EduFlow.Shared.Logging;

/// <summary>
/// One-liner Serilog setup so every service logs identically.
///
/// Usage in Program.cs:
///     builder.AddEduFlowSerilog();
///
/// Output:
///   - Console:   live log stream while running
///   - Logs/log-YYYYMMDD.txt:  rolling daily file, kept 30 days
///
/// Format: [2026-04-28 14:32:01 INF] HTTP GET /api/report → 200 in 47ms
///
/// Why a shared method:
/// M1 had Serilog inline in Program.cs, M3 had no Serilog at all, M8 had it
/// but with slightly different formatters. Now every service is identical.
/// </summary>
public static class SerilogExtensions
{
    public static WebApplicationBuilder AddEduFlowSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Service", builder.Environment.ApplicationName)
                .WriteTo.Console(
                    outputTemplate:
                        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] " +
                        "{Service} {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate:
                        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] " +
                        "{Service} [{TraceId}] {Message:lj}{NewLine}{Exception}");
        });

        return builder;
    }
}
