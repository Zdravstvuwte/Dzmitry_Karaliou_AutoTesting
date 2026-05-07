using System.Collections.Concurrent;
using System.Text.Json;

namespace WebUIAutomation.Tests.Infrastructure;

public static class TestExecutionSummary
{
    private static readonly ConcurrentBag<TestResultRecord> Records = [];

    public static void Add(TestResultRecord record)
    {
        Records.Add(record);
    }

    public static void WriteJsonSummary()
    {
        var payload = new
        {
            generatedAtUtc = DateTime.UtcNow,
            tests = Records.OrderBy(x => x.TestName).ToArray()
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(TestReportManager.JsonSummaryPath, json);
    }
}

public sealed record TestResultRecord(
    string TestName,
    string Status,
    double DurationMs,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset FinishedAtUtc,
    string? ErrorMessage,
    string? ScreenshotPath);
