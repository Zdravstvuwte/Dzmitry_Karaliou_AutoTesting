using Xunit;

namespace WebUIAutomation.Tests;

/// <summary>
/// Общий объект коллекции: демонстрация ICollectionFixture (suite-уровень для тестов в одной коллекции).
/// </summary>
public sealed class SuiteHooks : IDisposable
{
    public SuiteHooks()
    {
        StartedAtUtc = DateTime.UtcNow;
    }

    public DateTime StartedAtUtc { get; }

    public void Dispose()
    {
        // точка завершения жизненного цикла коллекции
    }
}

[CollectionDefinition("EhuSuiteHooks")]
public class EhuSuiteHooksCollection : ICollectionFixture<SuiteHooks>
{
}
