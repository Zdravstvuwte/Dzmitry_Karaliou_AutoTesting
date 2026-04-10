using NUnit.Framework;
using WebUIAutomation.Tests.Pages;

namespace WebUIAutomation.Tests;

[TestFixture]
[Category("Smoke")]
public class AboutPageNavigationTests : SeleniumTestBase
{
    [Test]
    public void AboutPage_ShouldOpen_WhenUserClicksAboutTab()
    {
        var homePage = new HomePage(Driver).Open();
        var aboutPage = homePage.OpenAbout();
        var redirected = aboutPage.IsOpened(TimeSpan.FromSeconds(10));
        Assert.That(redirected, Is.True, "Не дождались перехода на страницу About.");
        Assert.That(Driver.Url, Does.Contain("/about"));
        var pageHeaderFound = aboutPage.HasExpectedHeader(TimeSpan.FromSeconds(10));

        Assert.That(pageHeaderFound, Is.True, "Заголовок страницы не содержит 'About'.");
    }
}
