namespace WebUIAutomation.Tests;

/// <summary>
/// Настройки, создаваемые один раз на тестовый класс (IClassFixture).
/// </summary>
public sealed record TestSettings
{
    public string BaseUrl { get; init; } = "https://en.ehu.lt/";

    public TimeSpan DefaultImplicitWait { get; init; } = TimeSpan.FromSeconds(10);

    public TimeSpan? DefaultPageLoad { get; init; }
}
