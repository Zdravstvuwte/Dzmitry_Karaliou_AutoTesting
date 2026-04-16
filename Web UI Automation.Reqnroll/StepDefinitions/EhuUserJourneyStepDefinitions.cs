using System.Text.RegularExpressions;
using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;
using WebUIAutomation.Bdd.Support;
using WebUIAutomation.Tests.Pages;

namespace WebUIAutomation.Bdd.StepDefinitions;

[Binding]
public sealed class EhuUserJourneyStepDefinitions
{
    private readonly ScenarioContext _scenario;

    public EhuUserJourneyStepDefinitions(ScenarioContext scenario)
    {
        _scenario = scenario;
    }

    private IWebDriver Driver => _scenario.Get<IWebDriver>(BddKeys.WebDriver);

    [Given("the user opens the English home page")]
    public void GivenTheUserOpensTheEnglishHomePage()
    {
        var homePage = new HomePage(Driver).Open();
        Assert.That(homePage.IsLoaded(TimeSpan.FromSeconds(30)), Is.True, "Home page did not load within the timeout.");
    }

    [When("the user opens About from the home page")]
    public void WhenTheUserOpensAboutFromTheHomePage()
    {
        _ = new HomePage(Driver).OpenAbout();
    }

    [Then("the About page should be displayed with the expected heading")]
    public void ThenTheAboutPageShouldBeDisplayedWithTheExpectedHeading()
    {
        var aboutPage = new AboutPage(Driver);
        Assert.That(aboutPage.IsOpened(TimeSpan.FromSeconds(10)), Is.True, "Did not navigate to the About URL.");
        Assert.That(Driver.Url, Does.Contain("/about"));
        Assert.That(aboutPage.HasExpectedHeader(TimeSpan.FromSeconds(10)), Is.True, "The About H1 was not found or does not contain 'About'.");
    }

    [When("the user returns to the English home page")]
    public void WhenTheUserReturnsToTheEnglishHomePage()
    {
        _ = new HomePage(Driver).Open();
    }

    [When("the user searches for {string}")]
    public void WhenTheUserSearchesFor(string searchTerm)
    {
        Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(90);
        _ = new HomePage(Driver).Search(searchTerm);
    }

    [Then("the search results should reflect the query {string}")]
    public void ThenTheSearchResultsShouldReflectTheQuery(string searchTerm)
    {
        var resultsPage = new SearchResultsPage(Driver);
        Assert.That(resultsPage.HasQueryInUrl(searchTerm, TimeSpan.FromSeconds(25)), Is.True, "Search URL does not contain the expected query tokens.");
        Assert.That(resultsPage.ShowsResultsContext(TimeSpan.FromSeconds(20)), Is.True, "Search results context was not displayed.");
    }

    [When("the user opens the Contacts page")]
    public void WhenTheUserOpensTheContactsPage()
    {
        Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
        _ = new ContactsPage(Driver).Open();
    }

    [Then("the contacts page should list inquiry emails and phone numbers")]
    public void ThenTheContactsPageShouldListInquiryEmailsAndPhoneNumbers()
    {
        var contactsPage = new ContactsPage(Driver);
        Assert.That(contactsPage.IsLoaded(TimeSpan.FromSeconds(25)), Is.True, "Contacts page did not load.");
        Assert.That(Driver.Url, Does.Contain("/contacts").IgnoreCase);
        Assert.That(contactsPage.HasContactHeader(TimeSpan.FromSeconds(15)), Is.True, "Expected a Contact heading on the page.");

        var body = contactsPage.BodyText();
        Assert.That(body, Does.Contain("office@ehu.lt"));
        Assert.That(body, Does.Contain("consult@ehu.lt").Or.Contains("recruitment@ehu.lt"));

        var hasMainLandline = Regex.IsMatch(body, @"\+?\s*370\s*5\s*263\s*9650");
        var hasMobileAdmission = Regex.IsMatch(body, @"\+?\s*370\s*\(?644\)?\s*96\s*317");
        Assert.That(hasMainLandline || hasMobileAdmission, Is.True, "Expected Lithuanian phone number pattern was not found in page text.");
    }

    [When("the user switches the site language to Lithuanian")]
    public void WhenTheUserSwitchesTheSiteLanguageToLithuanian()
    {
        var homePage = new HomePage(Driver);
        Assert.That(homePage.IsLanguageSwitcherVisible(TimeSpan.FromSeconds(20)), Is.True, "Language switcher was not found.");
        homePage.SwitchToLithuanian();
    }

    [Then("the Lithuanian site version should be active")]
    public void ThenTheLithuanianSiteVersionShouldBeActive()
    {
        var switched = SeleniumWait.Until(
            () =>
            {
                var url = Driver.Url;
                return url.Contains("lt.ehuniversity", StringComparison.OrdinalIgnoreCase)
                    || url.Contains("lt.ehu", StringComparison.OrdinalIgnoreCase);
            },
            TimeSpan.FromSeconds(25));

        Assert.That(switched, Is.True, "Did not navigate to the Lithuanian site host.");

        var langLt = SeleniumWait.Until(
            () =>
            {
                var lang = Driver.FindElement(By.TagName("html")).GetAttribute("lang");
                return !string.IsNullOrEmpty(lang)
                    && lang.StartsWith("lt", StringComparison.OrdinalIgnoreCase);
            },
            TimeSpan.FromSeconds(15));

        Assert.That(langLt, Is.True, "The <html lang> attribute does not indicate Lithuanian.");

        var lang = Driver.FindElement(By.TagName("html")).GetAttribute("lang");
        Assert.That(lang, Does.StartWith("lt").IgnoreCase);
    }
}
