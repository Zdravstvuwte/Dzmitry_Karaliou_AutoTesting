using NUnit.Framework;
using OpenQA.Selenium;

namespace WebUIAutomation.Tests;

[TestFixture]
[Category("Regression")]
public class LanguageChangeTests : SeleniumTestBase
{
    protected override TimeSpan? PageLoadTimeout => TimeSpan.FromSeconds(60);

    [Test]
    public void Site_ShouldSwitchToLithuanian_WhenUserSelectsLtInLanguageSwitcher()
    {
        Driver.Navigate().GoToUrl("https://en.ehu.lt/");

        var switcherRoot = WaitUntil(
            () => Driver.FindElements(By.CssSelector("ul.language-switcher")).Count > 0,
            TimeSpan.FromSeconds(20));
        Assert.That(switcherRoot, Is.True, "Переключатель языка не найден.");

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

        Assert.That(ltLink, Is.Not.Null, "Ссылка на литовскую версию не найдена в переключателе языка.");
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", ltLink);

        var switched = WaitUntil(
            () =>
            {
                var url = Driver.Url;
                return url.Contains("lt.ehuniversity", StringComparison.OrdinalIgnoreCase)
                    || url.Contains("lt.ehu", StringComparison.OrdinalIgnoreCase);
            },
            TimeSpan.FromSeconds(25));

        Assert.That(switched, Is.True, "Не дождались перехода на литовскую версию сайта.");

        var langLt = WaitUntil(
            () =>
            {
                var lang = Driver.FindElement(By.TagName("html")).GetAttribute("lang");
                return !string.IsNullOrEmpty(lang)
                    && lang.StartsWith("lt", StringComparison.OrdinalIgnoreCase);
            },
            TimeSpan.FromSeconds(15));

        Assert.That(langLt, Is.True, "Атрибут lang у <html> не указывает на литовский язык.");
        Assert.That(
            Driver.FindElement(By.TagName("html")).GetAttribute("lang"),
            Does.StartWith("lt").IgnoreCase);
    }
}
