using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace WebUIAutomation.Tests.Infrastructure;

public static class TestReportManager
{
    private static readonly object SyncRoot = new();
    private static readonly ExtentReports Report = new();
    private static readonly AsyncLocal<ExtentTest?> CurrentTest = new();
    private static bool _initialized;

    public static string HtmlReportPath { get; private set; } = string.Empty;
    public static string JsonSummaryPath { get; private set; } = string.Empty;

    public static void Initialize()
    {
        lock (SyncRoot)
        {
            if (_initialized)
            {
                return;
            }

            HtmlReportPath = Path.Combine(TestRunPaths.ReportsDirectory, "extent-report.html");
            JsonSummaryPath = Path.Combine(TestRunPaths.ReportsDirectory, "test-summary.json");

            var spark = new ExtentSparkReporter(HtmlReportPath);
            Report.AttachReporter(spark);
            Report.AddSystemInfo(".NET", Environment.Version.ToString());
            Report.AddSystemInfo("OS", Environment.OSVersion.ToString());
            Report.AddSystemInfo("Machine", Environment.MachineName);
            Report.AddSystemInfo("Run started (UTC)", DateTime.UtcNow.ToString("O"));

            _initialized = true;
        }
    }

    public static void StartTest(string testName)
    {
        CurrentTest.Value = Report.CreateTest(testName);
    }

    public static void Log(Status status, string message)
    {
        CurrentTest.Value?.Log(status, message);
    }

    public static void AddScreenshot(string screenshotPath)
    {
        CurrentTest.Value?.AddScreenCaptureFromPath(screenshotPath);
    }

    public static void Flush()
    {
        Report.Flush();
    }
}
