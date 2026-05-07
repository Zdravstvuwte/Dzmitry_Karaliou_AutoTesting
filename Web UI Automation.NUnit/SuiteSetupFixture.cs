using NUnit.Framework;
using Serilog;
using WebUIAutomation.Tests.Infrastructure;

namespace WebUIAutomation.Tests;

/// <summary>
/// Однократная инициализация/завершение для всех тестов в пространстве имён (аналог suite setup/teardown).
/// </summary>
[SetUpFixture]
public class SuiteSetupFixture
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        TestLogger.Initialize();
        TestReportManager.Initialize();

        Log.Information("Test suite started at {StartedUtc}", DateTime.UtcNow);
        TestContext.Out.WriteLine($"[NUnit Suite] Старт прогона: {DateTime.UtcNow:O}");
        TestContext.Out.WriteLine($"[Artifacts] Root: {TestRunPaths.Root}");
    }

    [OneTimeTearDown]
    public void RunAfterAllTests()
    {
        var total = TestContext.CurrentContext.Result.FailCount + TestContext.CurrentContext.Result.PassCount;
        if (TestContext.CurrentContext.Result.FailCount > 0)
        {
            Log.Fatal("Test suite finished with failures. Passed: {Passed}; Failed: {Failed}", TestContext.CurrentContext.Result.PassCount, TestContext.CurrentContext.Result.FailCount);
        }
        else
        {
            Log.Information("Test suite finished successfully. Total tests: {Total}", total);
        }

        TestExecutionSummary.WriteJsonSummary();
        TestReportManager.Flush();
        Log.CloseAndFlush();

        TestContext.Out.WriteLine($"[NUnit Suite] Конец прогона: {DateTime.UtcNow:O}");
        TestContext.Out.WriteLine($"[Report] Extent HTML: {TestReportManager.HtmlReportPath}");
        TestContext.Out.WriteLine($"[Report] JSON summary: {TestReportManager.JsonSummaryPath}");
        TestContext.Out.WriteLine($"[Logs] Serilog file: {TestLogger.LogFilePath}");
    }
}
