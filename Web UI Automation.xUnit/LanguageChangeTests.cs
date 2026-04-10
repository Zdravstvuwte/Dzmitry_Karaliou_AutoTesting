using OpenQA.Selenium;
using Xunit;
using WebUIAutomation.Tests.Pages;

namespace WebUIAutomation.Tests;

[Trait("Category", "Regression")]
public class LanguageChangeTests : SeleniumTestBase, IClassFixture<TestSettings>
{
    public LanguageChangeTests(TestSettings settings)
        : base(settings with { DefaultPageLoad = TimeSpan.FromSeconds(60) })
    {
    }

    [Fact]
    public void Site_ShouldSwitchToLithuanian_WhenUserSelectsLtInLanguageSwitcher()
    {
        var homePage = new HomePage(Driver).Open();
        var switcherRoot = homePage.IsLanguageSwitcherVisible(TimeSpan.FromSeconds(20));
        Assert.True(switcherRoot, "Переключатель языка не найден.");
        homePage.SwitchToLithuanian();

        var switched = WaitUntil(
            () =>
            {
                var url = Driver.Url;
                return url.Contains("lt.ehuniversity", StringComparison.OrdinalIgnoreCase)
                    || url.Contains("lt.ehu", StringComparison.OrdinalIgnoreCase);
            },
            TimeSpan.FromSeconds(25));

        Assert.True(switched, "Не дождались перехода на литовскую версию сайта.");

        var langLt = WaitUntil(
            () =>
            {
                var lang = Driver.FindElement(By.TagName("html")).GetAttribute("lang");
                return !string.IsNullOrEmpty(lang)
                    && lang.StartsWith("lt", StringComparison.OrdinalIgnoreCase);
            },
            TimeSpan.FromSeconds(15));

        Assert.True(langLt, "Атрибут lang у <html> не указывает на литовский язык.");
        var lang = Driver.FindElement(By.TagName("html")).GetAttribute("lang");
        Assert.StartsWith("lt", lang, StringComparison.OrdinalIgnoreCase);
    }
}
