using OpenQA.Selenium;

namespace WebUIAutomation.Tests.Pages;

public sealed class AboutPage(IWebDriver driver) : BasePage(driver)
{
    public bool IsOpened(TimeSpan timeout)
    {
        return WaitUntil(() => Driver.Url.Contains("/about"), timeout);
    }

    public bool HasExpectedHeader(TimeSpan timeout)
    {
        return WaitUntil(
            () => Driver.FindElement(By.TagName("h1")).Text.Contains("About", StringComparison.OrdinalIgnoreCase),
            timeout);
    }
}
