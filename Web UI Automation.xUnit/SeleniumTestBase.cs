using OpenQA.Selenium;

namespace WebUIAutomation.Tests;

/// <summary>
/// Базовый класс: настройки из IClassFixture на конкретном классе; новый WebDriver на каждый тест.
/// </summary>
public abstract class SeleniumTestBase : IDisposable
{
    protected readonly IWebDriver Driver;

    protected SeleniumTestBase(TestSettings settings)
    {
        Settings = settings;
        Driver = WebDriverSingleton.Instance.CreateDriver(settings.DefaultImplicitWait, settings.DefaultPageLoad);
    }

    protected TestSettings Settings { get; }

    public void Dispose()
    {
        WebDriverSingleton.Instance.QuitAndDisposeDriver(Driver);
        GC.SuppressFinalize(this);
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
