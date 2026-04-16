using OpenQA.Selenium;
using Reqnroll;
using WebUIAutomation.Bdd.Support;
using WebUIAutomation.Tests;

namespace WebUIAutomation.Bdd.Hooks;

[Binding]
public sealed class WebDriverHooks
{
    private readonly ScenarioContext _scenario;

    public WebDriverHooks(ScenarioContext scenario)
    {
        _scenario = scenario;
    }

    [BeforeScenario]
    public void CreateDriver()
    {
        var driver = WebDriverSingleton.Instance.CreateDriver(
            implicitWait: TimeSpan.FromSeconds(10),
            pageLoadTimeout: TimeSpan.FromSeconds(60));
        _scenario[BddKeys.WebDriver] = driver;
    }

    [AfterScenario(Order = 0)]
    public void QuitDriver()
    {
        if (_scenario.TryGetValue(BddKeys.WebDriver, out IWebDriver? driver))
        {
            WebDriverSingleton.Instance.QuitAndDisposeDriver(driver);
        }
    }
}
