using OpenQA.Selenium;

namespace WebUIAutomation.Tests.Pages;

public sealed class ContactsPage(IWebDriver driver) : BasePage(driver)
{
    private const string Url = "https://en.ehu.lt/contacts/";

    public ContactsPage Open()
    {
        Driver.Navigate().GoToUrl(Url);
        return this;
    }

    public bool IsLoaded(TimeSpan timeout)
    {
        return WaitUntil(
            () => Driver.FindElements(By.TagName("body")).Count > 0
                && Driver.FindElement(By.TagName("body")).Text.Length > 100,
            timeout);
    }

    public bool HasContactHeader(TimeSpan timeout)
    {
        return WaitUntil(
            () =>
            {
                var h1 = Driver.FindElements(By.CssSelector("h1.subheader__title, h1"));
                return h1.Count > 0 && h1[0].Text.Contains("Contact", StringComparison.OrdinalIgnoreCase);
            },
            timeout);
    }

    public string BodyText()
    {
        return Driver.FindElement(By.TagName("body")).Text;
    }
}
