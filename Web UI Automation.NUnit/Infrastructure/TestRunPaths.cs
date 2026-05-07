using NUnit.Framework;

namespace WebUIAutomation.Tests.Infrastructure;

public static class TestRunPaths
{
    private static readonly string RunStamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
    private static readonly string OutputRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "artifacts", "nunit", RunStamp));

    static TestRunPaths()
    {
        Directory.CreateDirectory(OutputRoot);
    }

    public static string Root => OutputRoot;

    public static string LogsDirectory => EnsureDirectory("logs");

    public static string ReportsDirectory => EnsureDirectory("reports");

    public static string ScreenshotsDirectory => EnsureDirectory("screenshots");

    private static string EnsureDirectory(string name)
    {
        var path = Path.Combine(OutputRoot, name);
        Directory.CreateDirectory(path);
        return path;
    }
}
