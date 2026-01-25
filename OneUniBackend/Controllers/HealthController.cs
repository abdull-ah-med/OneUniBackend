using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OneUniBackend.Data;
using System.Reflection;

namespace OneUniBackend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly IWebHostEnvironment _environment;
    private static readonly DateTime _startTime = DateTime.UtcNow;

    public HealthController(HealthCheckService healthCheckService, IWebHostEnvironment environment)
    {
        _healthCheckService = healthCheckService;
        _environment = environment;
    }

    [HttpGet]
    [ProducesResponseType(typeof(BasicHealthResponse), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new BasicHealthResponse
        {
            Status = "healthy",
            Timestamp = DateTime.UtcNow,
            Service = "OneUni API",
            Version = GetAssemblyVersion()
        });
    }

    [HttpGet("detailed")]
    [ProducesResponseType(typeof(DetailedHealthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DetailedHealthResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetDetailed(CancellationToken cancellationToken)
    {
        var healthReport = await _healthCheckService.CheckHealthAsync(cancellationToken);
        
        var response = new DetailedHealthResponse
        {
            Status = healthReport.Status.ToString().ToLowerInvariant(),
            Timestamp = DateTime.UtcNow,
            Service = "OneUni API",
            Version = GetAssemblyVersion(),
            Environment = GetEnvironmentName(),
            Uptime = GetUptime(),
            Checks = healthReport.Entries.Select(entry => new HealthCheckResult
            {
                Name = entry.Key,
                Status = entry.Value.Status.ToString().ToLowerInvariant(),
                Duration = $"{entry.Value.Duration.TotalMilliseconds:F2}ms",
                Description = GetSafeDescription(entry.Value)
            }).ToList(),
            System = new SystemInfo
            {
                Runtime = $".NET {Environment.Version}",
                OS = GetSafeOSDescription(),
                ProcessorCount = Environment.ProcessorCount,
                MemoryStatus = GetMemoryStatus()
            }
        };

        var statusCode = healthReport.Status == HealthStatus.Healthy 
            ? StatusCodes.Status200OK 
            : StatusCodes.Status503ServiceUnavailable;

        return StatusCode(statusCode, response);
    }
    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetLiveness()
    {
        return Ok(new
        {
            status = "alive",
            timestamp = DateTime.UtcNow
        });
    }
    [HttpGet("ready")]
    [ProducesResponseType(typeof(ReadinessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReadinessResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetReadiness(CancellationToken cancellationToken)
    {
        var healthReport = await _healthCheckService.CheckHealthAsync(cancellationToken);
        
        var isReady = healthReport.Status == HealthStatus.Healthy;
        
        var response = new ReadinessResponse
        {
            Ready = isReady,
            Status = healthReport.Status.ToString().ToLowerInvariant(),
            Timestamp = DateTime.UtcNow,
            Dependencies = healthReport.Entries.ToDictionary(
                entry => entry.Key,
                entry => entry.Value.Status.ToString().ToLowerInvariant()
            )
        };

        return isReady 
            ? Ok(response) 
            : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }

    /// <summary>
    /// Startup probe - indicates if the application has finished starting
    /// </summary>
    [HttpGet("startup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetStartup()
    {
        return Ok(new
        {
            started = true,
            startTime = _startTime,
            timestamp = DateTime.UtcNow
        });
    }

    #region Private Helper Methods

    private static string GetAssemblyVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        return version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "1.0.0";
    }

    private string GetEnvironmentName()
    {
        // Return a safe environment indicator without revealing internal details
        return _environment.IsProduction() ? "production" 
             : _environment.IsStaging() ? "staging" 
             : "development";
    }

    private static UptimeInfo GetUptime()
    {
        var uptime = DateTime.UtcNow - _startTime;
        return new UptimeInfo
        {
            StartedAt = _startTime,
            Duration = FormatUptime(uptime),
            TotalSeconds = (long)uptime.TotalSeconds
        };
    }

    private static string FormatUptime(TimeSpan uptime)
    {
        if (uptime.TotalDays >= 1)
            return $"{(int)uptime.TotalDays}d {uptime.Hours}h {uptime.Minutes}m";
        if (uptime.TotalHours >= 1)
            return $"{(int)uptime.TotalHours}h {uptime.Minutes}m {uptime.Seconds}s";
        if (uptime.TotalMinutes >= 1)
            return $"{(int)uptime.TotalMinutes}m {uptime.Seconds}s";
        return $"{uptime.Seconds}s";
    }

    private static string GetSafeDescription(HealthReportEntry entry)
    {
        // Don't expose exception details or sensitive information
        return entry.Status switch
        {
            HealthStatus.Healthy => "Operating normally",
            HealthStatus.Degraded => "Operating with reduced performance",
            HealthStatus.Unhealthy => "Service unavailable",
            _ => "Unknown status"
        };
    }

    private static string GetSafeOSDescription()
    {
        // Return OS family without detailed version info that could aid attackers
        if (OperatingSystem.IsWindows()) return "Windows";
        if (OperatingSystem.IsLinux()) return "Linux";
        if (OperatingSystem.IsMacOS()) return "macOS";
        return "Unknown";
    }

    private static string GetMemoryStatus()
    {
        // Return a relative memory status without exact figures
        var workingSet = Environment.WorkingSet;
        var memoryMB = workingSet / (1024 * 1024);
        
        return memoryMB switch
        {
            < 256 => "low",
            < 512 => "normal",
            < 1024 => "moderate",
            _ => "high"
        };
    }

    #endregion
}

#region Response DTOs

public class BasicHealthResponse
{
    public string Status { get; set; } = default!;
    public DateTime Timestamp { get; set; }
    public string Service { get; set; } = default!;
    public string Version { get; set; } = default!;
}

public class DetailedHealthResponse
{
    public string Status { get; set; } = default!;
    public DateTime Timestamp { get; set; }
    public string Service { get; set; } = default!;
    public string Version { get; set; } = default!;
    public string Environment { get; set; } = default!;
    public UptimeInfo Uptime { get; set; } = default!;
    public List<HealthCheckResult> Checks { get; set; } = new();
    public SystemInfo System { get; set; } = default!;
}

public class UptimeInfo
{
    public DateTime StartedAt { get; set; }
    public string Duration { get; set; } = default!;
    public long TotalSeconds { get; set; }
}

public class HealthCheckResult
{
    public string Name { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Duration { get; set; } = default!;
    public string Description { get; set; } = default!;
}

public class SystemInfo
{
    public string Runtime { get; set; } = default!;
    public string OS { get; set; } = default!;
    public int ProcessorCount { get; set; }
    public string MemoryStatus { get; set; } = default!;
}

public class ReadinessResponse
{
    public bool Ready { get; set; }
    public string Status { get; set; } = default!;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string> Dependencies { get; set; } = new();
}

#endregion
