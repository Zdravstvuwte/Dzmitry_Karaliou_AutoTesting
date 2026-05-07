using NUnit.Framework;
using Shouldly;
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
        redirected.ShouldBeTrue("Не дождались перехода на страницу About.");
        Driver.Url.ShouldContain("/about");
        var pageHeaderFound = aboutPage.HasExpectedHeader(TimeSpan.FromSeconds(10));

        pageHeaderFound.ShouldBeTrue("Заголовок страницы не содержит 'About'.");
    }
}
