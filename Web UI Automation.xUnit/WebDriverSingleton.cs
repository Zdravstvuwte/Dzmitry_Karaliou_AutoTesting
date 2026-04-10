using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WebUIAutomation.Tests;

/// <summary>
/// Singleton manager for one active WebDriver per test.
/// </summary>
public sealed class WebDriverSingleton
{
    private static readonly Lazy<WebDriverSingleton> LazyInstance = new(() => new WebDriverSingleton());

    private WebDriverSingleton()
    {
    }

    public static WebDriverSingleton Instance => LazyInstance.Value;

    public IWebDriver CreateDriver(TimeSpan implicitWait, TimeSpan? pageLoadTimeout = null)
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.PageLoadStrategy = PageLoadStrategy.Eager;

        var service = ChromeDriverService.CreateDefaultService();
        var driver = new ChromeDriver(service, options, TimeSpan.FromMinutes(3));
        driver.Manage().Timeouts().ImplicitWait = implicitWait;
        if (pageLoadTimeout is { } pageLoad)
        {
            driver.Manage().Timeouts().PageLoad = pageLoad;
        }

        return driver;
    }

    public void QuitAndDisposeDriver(IWebDriver? driver)
    {
        if (driver is null)
        {
            return;
        }

        try
        {
            driver.Quit();
        }
        catch
        {
            // driver could already be closed by browser process
        }
        finally
        {
            driver.Dispose();
        }
    }
}
