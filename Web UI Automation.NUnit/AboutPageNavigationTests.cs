using NUnit.Framework;
using OpenQA.Selenium;

namespace WebUIAutomation.Tests;

[TestFixture]
[Category("Smoke")]
public class AboutPageNavigationTests : SeleniumTestBase
{
    [Test]
    public void AboutPage_ShouldOpen_WhenUserClicksAboutTab()
    {
        Driver.Navigate().GoToUrl("https://en.ehu.lt/");

        var aboutTab = Driver.FindElement(By.CssSelector("a[href*='/about']"));
        try
        {
            aboutTab.Click();
        }
        catch (ElementClickInterceptedException)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", aboutTab);
        }

        var redirected = WaitUntil(() => Driver.Url.Contains("/about"), TimeSpan.FromSeconds(10));
        Assert.That(redirected, Is.True, "Не дождались перехода на страницу About.");
        Assert.That(Driver.Url, Does.Contain("/about"));

        var pageHeaderFound = WaitUntil(
            () => Driver.FindElement(By.TagName("h1")).Text.Contains("About", StringComparison.OrdinalIgnoreCase),
            TimeSpan.FromSeconds(10));

        Assert.That(pageHeaderFound, Is.True, "Заголовок страницы не содержит 'About'.");
    }
}
