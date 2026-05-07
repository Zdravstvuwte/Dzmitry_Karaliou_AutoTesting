using NUnit.Framework;
using OpenQA.Selenium;
using Serilog;
using WebUIAutomation.Tests.Infrastructure;
using AventStack.ExtentReports;

namespace WebUIAutomation.Tests;

/// <summary>
/// Базовый класс: OneTimeSetUp/OneTimeTearDown на уровне фикстуры + SetUp/TearDown на каждый тест (драйвер).
/// </summary>
[TestFixture]
public abstract class SeleniumTestBase
{
    protected IWebDriver Driver = null!;
    private DateTimeOffset _startedAtUtc;

    protected virtual TimeSpan ImplicitWait => TimeSpan.FromSeconds(10);

    protected virtual TimeSpan? PageLoadTimeout => null;

    [OneTimeSetUp]
    public void FixtureOneTimeSetUp()
    {
        TestContext.Out.WriteLine($"[NUnit Class] Начало: {GetType().Name}");
    }

    [OneTimeTearDown]
    public void FixtureOneTimeTearDown()
    {
        TestContext.Out.WriteLine($"[NUnit Class] Конец: {GetType().Name}");
    }

    [SetUp]
    public void CreateDriverPerTest()
    {
        TestLogger.Initialize();
        TestReportManager.Initialize();

        _startedAtUtc = DateTimeOffset.UtcNow;
        var testName = TestContext.CurrentContext.Test.FullName;
        TestReportManager.StartTest(testName);
        TestReportManager.Log(Status.Info, $"Test started at {_startedAtUtc:O}");
        Log.Information("Starting test: {TestName}", testName);

        Driver = WebDriverSingleton.Instance.CreateDriver(ImplicitWait, PageLoadTimeout);
        Log.Debug("WebDriver created with implicit wait {ImplicitWait}s and page load timeout {PageLoadTimeout}s", ImplicitWait.TotalSeconds, PageLoadTimeout?.TotalSeconds);
    }

    [TearDown]
    public void QuitDriver()
    {
        var result = TestContext.CurrentContext.Result;
        var endedAtUtc = DateTimeOffset.UtcNow;
        string? screenshotPath = null;

        switch (result.Outcome.Status)
        {
            case NUnit.Framework.Interfaces.TestStatus.Passed:
                TestReportManager.Log(Status.Pass, "Test passed.");
                Log.Information("Test passed: {TestName}", TestContext.CurrentContext.Test.FullName);
                break;
            case NUnit.Framework.Interfaces.TestStatus.Failed:
                screenshotPath = TryTakeScreenshot();
                TestReportManager.Log(Status.Fail, result.Message);
                if (!string.IsNullOrWhiteSpace(screenshotPath))
                {
                    TestReportManager.AddScreenshot(screenshotPath);
                }

                Log.Error("Test failed: {TestName}. Error: {Error}", TestContext.CurrentContext.Test.FullName, result.Message);
                break;
            case NUnit.Framework.Interfaces.TestStatus.Skipped:
            case NUnit.Framework.Interfaces.TestStatus.Inconclusive:
                TestReportManager.Log(Status.Warning, $"{result.Outcome.Status}: {result.Message}");
                Log.Warning("Test not executed fully: {TestName}. Status: {Status}", TestContext.CurrentContext.Test.FullName, result.Outcome.Status);
                break;
        }

        var duration = (endedAtUtc - _startedAtUtc).TotalMilliseconds;
        TestExecutionSummary.Add(
            new TestResultRecord(
                TestContext.CurrentContext.Test.FullName,
                result.Outcome.Status.ToString(),
                duration,
                _startedAtUtc,
                endedAtUtc,
                string.IsNullOrWhiteSpace(result.Message) ? null : result.Message,
                screenshotPath));

        WebDriverSingleton.Instance.QuitAndDisposeDriver(Driver);
        TestReportManager.Flush();
        Log.Debug("WebDriver disposed for test: {TestName}", TestContext.CurrentContext.Test.FullName);
    }

    private string? TryTakeScreenshot()
    {
        try
        {
            if (Driver is not ITakesScreenshot screenshotDriver)
            {
                return null;
            }

            var screenshot = screenshotDriver.GetScreenshot();
            var safeName = string.Join("_", TestContext.CurrentContext.Test.Name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
            var path = Path.Combine(TestRunPaths.ScreenshotsDirectory, $"{safeName}_{DateTime.UtcNow:yyyyMMdd-HHmmss}.png");
            screenshot.SaveAsFile(path);
            Log.Warning("Failure screenshot saved: {ScreenshotPath}", path);
            return path;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Could not capture failure screenshot.");
            return null;
        }
    }

    protected static bool WaitUntil(Func<bool> condition, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                if (condition())
                {
                    return true;
                }
            }
            catch
            {
                // временные сбои DOM/сети
            }

            Thread.Sleep(200);
        }

        return false;
    }

    protected static IWebElement? WaitUntilElement(Func<IWebElement?> factory, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            var el = factory();
            if (el is not null)
            {
                return el;
            }

            Thread.Sleep(200);
        }

        return null;
    }
}
