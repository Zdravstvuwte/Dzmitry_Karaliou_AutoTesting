using System.Collections;
using NUnit.Framework;
using Shouldly;
using WebUIAutomation.Tests.Pages;

namespace WebUIAutomation.Tests;

/// <summary>
/// Data-driven сценарии через TestCaseSource.
/// </summary>
[TestFixture]
[Category("Data")]
public class HomePageParameterizedTests : SeleniumTestBase
{
    private static IEnumerable TopLevelPages()
    {
        yield return new TestCaseData("https://en.ehu.lt/", "European")
            .SetArgDisplayNames("home", "title-fragment");
        yield return new TestCaseData("https://en.ehu.lt/about/", "About")
            .SetArgDisplayNames("about", "title-fragment");
        yield return new TestCaseData("https://en.ehu.lt/contacts/", "Contact")
            .SetArgDisplayNames("contacts", "title-fragment");
    }

    [Test]
    [Category("Smoke")]
    [TestCaseSource(nameof(TopLevelPages))]
    public void Page_ShouldLoad_AndTitleContains(string url, string titleFragment)
    {
        Driver.Navigate().GoToUrl(url);
        var homePage = new HomePage(Driver);
        var loaded = homePage.IsLoaded(TimeSpan.FromSeconds(30));

        loaded.ShouldBeTrue($"Страница не загрузилась: {url}");
        Driver.Title.ToLowerInvariant().ShouldContain(titleFragment.ToLowerInvariant());
    }
}
