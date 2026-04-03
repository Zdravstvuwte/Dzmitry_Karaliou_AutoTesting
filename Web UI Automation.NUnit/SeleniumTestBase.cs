using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WebUIAutomation.Tests;

/// <summary>
/// Базовый класс: OneTimeSetUp/OneTimeTearDown на уровне фикстуры + SetUp/TearDown на каждый тест (драйвер).
/// </summary>
[TestFixture]
public abstract class SeleniumTestBase
{
    protected IWebDriver Driver = null!;

    protected virtual TimeSpan ImplicitWait => TimeSpan.FromSeconds(10);

    protected virtual TimeSpan? PageLoadTimeout => null;

    [OneTimeSetUp]
    public void FixtureOneTimeSetUp()
    {
        TestContext.Out.WriteLine($"[NUnit Class] Начало: {GetType().Name}");
    }

    [OneTimeTearDown]
    public void FixtureOneTimeTearDown()
    {
        TestContext.Out.WriteLine($"[NUnit Class] Конец: {GetType().Name}");
    }

    [SetUp]
    public void CreateDriverPerTest()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.PageLoadStrategy = PageLoadStrategy.Eager;

        var service = ChromeDriverService.CreateDefaultService();
        Driver = new ChromeDriver(service, options, TimeSpan.FromMinutes(3));
        Driver.Manage().Timeouts().ImplicitWait = ImplicitWait;
        if (PageLoadTimeout is { } pageLoad)
        {
            Driver.Manage().Timeouts().PageLoad = pageLoad;
        }
    }

    [TearDown]
    public void QuitDriver()
    {
        Driver.Quit();
    }

    protected static bool WaitUntil(Func<bool> condition, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                if (condition())
                {
                    return true;
                }
            }
            catch
            {
                // временные сбои DOM/сети
            }

            Thread.Sleep(200);
        }

        return false;
    }

    protected static IWebElement? WaitUntilElement(Func<IWebElement?> factory, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            var el = factory();
            if (el is not null)
            {
                return el;
            }

            Thread.Sleep(200);
        }

        return null;
    }
}
