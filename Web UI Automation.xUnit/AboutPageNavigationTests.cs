using OpenQA.Selenium;
using Xunit;

namespace WebUIAutomation.Tests;

[Trait("Category", "Smoke")]
public class AboutPageNavigationTests : SeleniumTestBase, IClassFixture<TestSettings>
{
    public AboutPageNavigationTests(TestSettings settings)
        : base(settings)
    {
    }

    [Fact]
    public void AboutPage_ShouldOpen_WhenUserClicksAboutTab()
    {
        Driver.Navigate().GoToUrl(Settings.BaseUrl);

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
        Assert.True(redirected, "Не дождались перехода на страницу About.");
        Assert.Contains("/about", Driver.Url);

        var pageHeaderFound = WaitUntil(
            () => Driver.FindElement(By.TagName("h1")).Text.Contains("About", StringComparison.OrdinalIgnoreCase),
            TimeSpan.FromSeconds(10));

        Assert.True(pageHeaderFound, "Заголовок страницы не содержит 'About'.");
    }
}
