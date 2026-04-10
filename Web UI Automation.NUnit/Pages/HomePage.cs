using OpenQA.Selenium;

namespace WebUIAutomation.Tests.Pages;

public sealed class HomePage(IWebDriver driver) : BasePage(driver)
{
    private const string BaseUrl = "https://en.ehu.lt/";

    public HomePage Open()
    {
        Driver.Navigate().GoToUrl(BaseUrl);
        return this;
    }

    public bool IsLoaded(TimeSpan timeout)
    {
        return WaitUntil(
            () => Driver.Title.Length > 0 && Driver.FindElements(By.TagName("body")).Count > 0,
            timeout);
    }

    public AboutPage OpenAbout()
    {
        var aboutTab = Driver.FindElement(By.CssSelector("a[href*='/about']"));
        try
        {
            aboutTab.Click();
        }
        catch (ElementClickInterceptedException)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", aboutTab);
        }

        return new AboutPage(Driver);
    }

    public bool IsLanguageSwitcherVisible(TimeSpan timeout)
    {
        return WaitUntil(
            () => Driver.FindElements(By.CssSelector("ul.language-switcher")).Count > 0,
            timeout);
    }

    public void SwitchToLithuanian()
    {
        var ltLink = WaitUntilElement(
            () =>
            {
                try
                {
                    return Driver.FindElement(
                        By.CssSelector("ul.language-switcher a[href*='lt.ehuniversity'], ul.language-switcher a[href*='lt.ehu']"));
                }
                catch
                {
                    return null;
                }
            },
            TimeSpan.FromSeconds(10));

        if (ltLink is null)
        {
            throw new InvalidOperationException("Lithuanian language link was not found.");
        }

        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", ltLink);
    }

    public SearchResultsPage Search(string searchTerm)
    {
        var formReady = WaitUntil(
            () => Driver.FindElements(By.CssSelector("form.header-search__form input[name='s']")).Count > 0,
            TimeSpan.FromSeconds(20));

        if (!formReady)
        {
            throw new InvalidOperationException("Header search form was not found.");
        }

        ((IJavaScriptExecutor)Driver).ExecuteScript(
            "var form=document.querySelector('form.header-search__form');" +
            "var input=form.querySelector('input[name=s]');" +
            "input.value=arguments[0];" +
            "form.submit();",
            searchTerm);

        return new SearchResultsPage(Driver);
    }
}
