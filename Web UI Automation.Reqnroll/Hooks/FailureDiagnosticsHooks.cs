using OpenQA.Selenium;
using Reqnroll;
using WebUIAutomation.Bdd.Support;

namespace WebUIAutomation.Bdd.Hooks;

/// <summary>
/// Emits structured diagnostics when a scenario fails (URL, title, exception) for easier triage.
/// </summary>
[Binding]
public sealed class FailureDiagnosticsHooks
{
    private readonly ScenarioContext _scenario;

    public FailureDiagnosticsHooks(ScenarioContext scenario)
    {
        _scenario = scenario;
    }

    [AfterScenario(Order = 10000)]
    public void LogFailureDetails()
    {
        if (_scenario.ScenarioExecutionStatus == ScenarioExecutionStatus.OK)
        {
            return;
        }

        if (!_scenario.TryGetValue(BddKeys.WebDriver, out IWebDriver? driver) || driver is null)
        {
            Console.WriteLine("[BDD] Scenario failed before a WebDriver instance was available.");
            return;
        }

        Console.WriteLine($"[BDD] Failure — URL: {driver.Url}");
        Console.WriteLine($"[BDD] Failure — Title: {driver.Title}");

        if (_scenario.TestError is { } error)
        {
            Console.WriteLine($"[BDD] Failure — {error.GetType().Name}: {error.Message}");
            if (!string.IsNullOrEmpty(error.StackTrace))
            {
                Console.WriteLine($"[BDD] Failure — Stack trace:{Environment.NewLine}{error.StackTrace}");
            }
        }
    }
}
