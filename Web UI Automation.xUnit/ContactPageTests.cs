using System.Text.RegularExpressions;
using OpenQA.Selenium;
using Xunit;

namespace WebUIAutomation.Tests;

[Trait("Category", "Smoke")]
public class ContactPageTests : SeleniumTestBase, IClassFixture<TestSettings>
{
    public ContactPageTests(TestSettings settings)
        : base(settings with { DefaultPageLoad = TimeSpan.FromSeconds(60) })
    {
    }

    [Fact]
    public void ContactsPage_ShouldShowEmailAndPhone_ForUniversityInquiries()
    {
        Driver.Navigate().GoToUrl("https://en.ehu.lt/contacts/");

        var loaded = WaitUntil(
            () => Driver.FindElements(By.TagName("body")).Count > 0
                && Driver.FindElement(By.TagName("body")).Text.Length > 100,
            TimeSpan.FromSeconds(25));
        Assert.True(loaded, "Страница контактов не загрузилась.");

        Assert.Contains("/contacts", Driver.Url, StringComparison.OrdinalIgnoreCase);

        var headingOk = WaitUntil(
            () =>
            {
                var h1 = Driver.FindElements(By.CssSelector("h1.subheader__title, h1"));
                return h1.Count > 0 && h1[0].Text.Contains("Contact", StringComparison.OrdinalIgnoreCase);
            },
            TimeSpan.FromSeconds(15));
        Assert.True(headingOk, "Ожидался заголовок страницы с «Contact».");

        var body = Driver.FindElement(By.TagName("body")).Text;

        Assert.Contains("office@ehu.lt", body);
        Assert.True(
            body.Contains("consult@ehu.lt", StringComparison.OrdinalIgnoreCase)
            || body.Contains("recruitment@ehu.lt", StringComparison.OrdinalIgnoreCase));

        var hasMainLandline = Regex.IsMatch(body, @"\+?\s*370\s*5\s*263\s*9650");
        var hasMobileAdmission = Regex.IsMatch(body, @"\+?\s*370\s*\(?644\)?\s*96\s*317");
        Assert.True(hasMainLandline || hasMobileAdmission, "На странице не найден ожидаемый номер телефона (+370 …).");
    }
}
