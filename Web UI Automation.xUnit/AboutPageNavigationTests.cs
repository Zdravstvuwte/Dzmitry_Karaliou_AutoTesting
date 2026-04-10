using Xunit;
using WebUIAutomation.Tests.Pages;

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
        var homePage = new HomePage(Driver).Open();
        var aboutPage = homePage.OpenAbout();
        var redirected = aboutPage.IsOpened(TimeSpan.FromSeconds(10));
        Assert.True(redirected, "Не дождались перехода на страницу About.");
        Assert.Contains("/about", Driver.Url);
        var pageHeaderFound = aboutPage.HasExpectedHeader(TimeSpan.FromSeconds(10));

        Assert.True(pageHeaderFound, "Заголовок страницы не содержит 'About'.");
    }
}
