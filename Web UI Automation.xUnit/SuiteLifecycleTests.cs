using Xunit;

namespace WebUIAutomation.Tests;

/// <summary>
/// Тесты в отдельной коллекции с ICollectionFixture — suite setup для этой коллекции.
/// </summary>
[Collection("EhuSuiteHooks")]
public class SuiteLifecycleTests
{
    private readonly SuiteHooks _hooks;

    public SuiteLifecycleTests(SuiteHooks hooks)
    {
        _hooks = hooks;
    }

    [Fact]
    [Trait("Category", "Smoke")]
    public void SuiteHooks_AreInitialized()
    {
        Assert.NotEqual(default, _hooks.StartedAtUtc);
    }
}
