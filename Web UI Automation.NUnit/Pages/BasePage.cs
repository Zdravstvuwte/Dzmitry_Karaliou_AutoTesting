using OpenQA.Selenium;

namespace WebUIAutomation.Tests.Pages;

public abstract class BasePage
{
    protected BasePage(IWebDriver driver)
    {
        Driver = driver;
    }

    protected IWebDriver Driver { get; }

    protected bool WaitUntil(Func<bool> condition, TimeSpan timeout)
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
                // temporary DOM/network instability
            }

            Thread.Sleep(200);
        }

        return false;
    }

    protected IWebElement? WaitUntilElement(Func<IWebElement?> factory, TimeSpan timeout)
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
