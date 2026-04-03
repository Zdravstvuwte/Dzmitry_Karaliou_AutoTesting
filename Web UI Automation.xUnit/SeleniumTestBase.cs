using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.PageLoadStrategy = PageLoadStrategy.Eager;

        var service = ChromeDriverService.CreateDefaultService();
        Driver = new ChromeDriver(service, options, TimeSpan.FromMinutes(3));
        Driver.Manage().Timeouts().ImplicitWait = settings.DefaultImplicitWait;
        if (settings.DefaultPageLoad is { } pageLoad)
        {
            Driver.Manage().Timeouts().PageLoad = pageLoad;
        }
    }

    protected TestSettings Settings { get; }

    public void Dispose()
    {
        Driver.Quit();
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
