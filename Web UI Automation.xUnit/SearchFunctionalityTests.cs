using Xunit;
using WebUIAutomation.Tests.Pages;

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
        var homePage = new HomePage(Driver).Open();
        var resultsPage = homePage.Search(searchTerm);
        var redirectedToSearch = resultsPage.HasQueryInUrl(searchTerm, TimeSpan.FromSeconds(25));

        Assert.True(redirectedToSearch, "После отправки поиска URL не содержит запрос study programs.");
        var hasSearchResultsContext = resultsPage.ShowsResultsContext(TimeSpan.FromSeconds(20));

        Assert.True(hasSearchResultsContext, "Страница результатов поиска не отобразилась.");
    }
}
