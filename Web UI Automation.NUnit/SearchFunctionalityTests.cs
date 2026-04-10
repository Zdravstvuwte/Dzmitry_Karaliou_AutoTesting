using NUnit.Framework;
using WebUIAutomation.Tests.Pages;

namespace WebUIAutomation.Tests;

[TestFixture]
[Category("Regression")]
public class SearchFunctionalityTests : SeleniumTestBase
{
    protected override TimeSpan ImplicitWait => TimeSpan.FromSeconds(5);

    [Test]
    public void Search_ShouldReturnResults_WhenSearchTermIsValid()
    {
        const string searchTerm = "study programs";

        Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(90);
        var homePage = new HomePage(Driver).Open();
        var resultsPage = homePage.Search(searchTerm);
        var redirectedToSearch = resultsPage.HasQueryInUrl(searchTerm, TimeSpan.FromSeconds(25));

        Assert.That(redirectedToSearch, Is.True, "После отправки поиска URL не содержит запрос study programs.");
        var hasSearchResultsContext = resultsPage.ShowsResultsContext(TimeSpan.FromSeconds(20));

        Assert.That(hasSearchResultsContext, Is.True, "Страница результатов поиска не отобразилась.");
    }
}
