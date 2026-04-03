using OpenQA.Selenium;
using Xunit;

namespace WebUIAutomation.Tests;

[Trait("Category", "Regression")]
public class SearchFunctionalityTests : SeleniumTestBase, IClassFixture<TestSettings>
{
    public SearchFunctionalityTests(TestSettings settings)
        : base(settings with { DefaultImplicitWait = TimeSpan.FromSeconds(5) })
    {
    }

    [Fact]
    public void Search_ShouldReturnResults_WhenSearchTermIsValid()
    {
        const string searchTerm = "study programs";

        Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(90);
        Driver.Navigate().GoToUrl("https://en.ehu.lt/");

        var formReady = WaitUntil(
            () => Driver.FindElements(By.CssSelector("form.header-search__form input[name='s']")).Count > 0,
            TimeSpan.FromSeconds(20));
        Assert.True(formReady, "Форма поиска в шапке не найдена на главной.");

        ((IJavaScriptExecutor)Driver).ExecuteScript(
            "var form=document.querySelector('form.header-search__form');" +
            "var input=form.querySelector('input[name=s]');" +
            "input.value=arguments[0];" +
            "form.submit();",
            searchTerm);

        var redirectedToSearch = WaitUntil(
            () =>
            {
                var url = Driver.Url;
                if (!url.Contains("?s=", StringComparison.OrdinalIgnoreCase)
                    && !url.Contains("&s=", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var decoded = Uri.UnescapeDataString(url);
                return decoded.Contains("study", StringComparison.OrdinalIgnoreCase)
                    && decoded.Contains("program", StringComparison.OrdinalIgnoreCase);
            },
            TimeSpan.FromSeconds(25));

        Assert.True(redirectedToSearch, "После отправки поиска URL не содержит запрос study programs.");

        var hasSearchResultsContext = WaitUntil(
            () =>
            {
                var pageText = Driver.FindElement(By.TagName("body")).Text;
                return pageText.Contains("You searched for", StringComparison.OrdinalIgnoreCase)
                    || pageText.Contains("Search results", StringComparison.OrdinalIgnoreCase)
                    || pageText.Contains("results found", StringComparison.OrdinalIgnoreCase);
            },
            TimeSpan.FromSeconds(20));

        Assert.True(hasSearchResultsContext, "Страница результатов поиска не отобразилась.");
    }
}
