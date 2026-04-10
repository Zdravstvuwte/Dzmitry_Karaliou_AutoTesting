using NUnit.Framework;
using OpenQA.Selenium;
using WebUIAutomation.Tests.Pages;

namespace WebUIAutomation.Tests;

[TestFixture]
[Category("Regression")]
public class LanguageChangeTests : SeleniumTestBase
{
    protected override TimeSpan? PageLoadTimeout => TimeSpan.FromSeconds(60);

    [Test]
    public void Site_ShouldSwitchToLithuanian_WhenUserSelectsLtInLanguageSwitcher()
    {
        var homePage = new HomePage(Driver).Open();
        var switcherRoot = homePage.IsLanguageSwitcherVisible(TimeSpan.FromSeconds(20));
        Assert.That(switcherRoot, Is.True, "Переключатель языка не найден.");
        homePage.SwitchToLithuanian();

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
