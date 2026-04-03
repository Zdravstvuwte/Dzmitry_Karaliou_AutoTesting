using OpenQA.Selenium;
using Xunit;

namespace WebUIAutomation.Tests;

/// <summary>
/// Data-driven сценарии: Theory + InlineData / MemberData.
/// </summary>
public class HomePageTheoryTests : SeleniumTestBase, IClassFixture<TestSettings>
{
    public HomePageTheoryTests(TestSettings settings)
        : base(settings)
    {
    }


    public static TheoryData<string, string> TitleCases => new()
    {
        { "https://en.ehu.lt/", "European" },
        { "https://en.ehu.lt/about/", "About" },
        { "https://en.ehu.lt/contacts/", "Contact" },
    };

    [Theory]
    [InlineData("https://en.ehu.lt/", "European")]
    [Trait("Category", "Data")]
    public void Home_ShouldHaveTitleContaining(string url, string titleFragment)
    {
        Driver.Navigate().GoToUrl(url);
        var loaded = WaitUntil(
            () => Driver.Title.Length > 0 && Driver.FindElements(By.TagName("body")).Count > 0,
            TimeSpan.FromSeconds(30));
        Assert.True(loaded);
        Assert.Contains(titleFragment, Driver.Title, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [MemberData(nameof(TitleCases))]
    [Trait("Category", "Data")]
    [Trait("Category", "Smoke")]
    public void Page_ShouldLoad_AndTitleContains(string url, string titleFragment)
    {
        Driver.Navigate().GoToUrl(url);

        var loaded = WaitUntil(
            () => Driver.Title.Length > 0 && Driver.FindElements(By.TagName("body")).Count > 0,
            TimeSpan.FromSeconds(30));

        Assert.True(loaded, $"Страница не загрузилась: {url}");
        Assert.Contains(titleFragment, Driver.Title, StringComparison.OrdinalIgnoreCase);
    }
}
