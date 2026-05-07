using Serilog;
using Serilog.Events;

namespace WebUIAutomation.Tests.Infrastructure;

public static class TestLogger
{
    private static readonly object SyncRoot = new();
    private static bool _initialized;
    public static string LogFilePath { get; private set; } = string.Empty;

    public static void Initialize()
    {
        lock (SyncRoot)
        {
            if (_initialized)
            {
                return;
            }

            LogFilePath = Path.Combine(TestRunPaths.LogsDirectory, "test-run.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(LogFilePath, rollingInterval: RollingInterval.Infinite)
                .CreateLogger();

            _initialized = true;
        }
    }
}
