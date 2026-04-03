using NUnit.Framework;

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
        TestContext.Out.WriteLine($"[NUnit Suite] Старт прогона: {DateTime.UtcNow:O}");
    }

    [OneTimeTearDown]
    public void RunAfterAllTests()
    {
        TestContext.Out.WriteLine($"[NUnit Suite] Конец прогона: {DateTime.UtcNow:O}");
    }
}
