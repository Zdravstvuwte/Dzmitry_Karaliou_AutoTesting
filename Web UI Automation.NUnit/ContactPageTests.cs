using System.Text.RegularExpressions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace WebUIAutomation.Tests;

[TestFixture]
[Category("Smoke")]
public class ContactPageTests : SeleniumTestBase
{
    protected override TimeSpan? PageLoadTimeout => TimeSpan.FromSeconds(60);

    [Test]
    public void ContactsPage_ShouldShowEmailAndPhone_ForUniversityInquiries()
    {
        Driver.Navigate().GoToUrl("https://en.ehu.lt/contacts/");

        var loaded = WaitUntil(
            () => Driver.FindElements(By.TagName("body")).Count > 0
                && Driver.FindElement(By.TagName("body")).Text.Length > 100,
            TimeSpan.FromSeconds(25));
        Assert.That(loaded, Is.True, "Страница контактов не загрузилась.");

        Assert.That(Driver.Url, Does.Contain("/contacts").IgnoreCase);

        var headingOk = WaitUntil(
            () =>
            {
                var h1 = Driver.FindElements(By.CssSelector("h1.subheader__title, h1"));
                return h1.Count > 0 && h1[0].Text.Contains("Contact", StringComparison.OrdinalIgnoreCase);
            },
            TimeSpan.FromSeconds(15));
        Assert.That(headingOk, Is.True, "Ожидался заголовок страницы с «Contact».");

        var body = Driver.FindElement(By.TagName("body")).Text;

        Assert.That(body, Does.Contain("office@ehu.lt"));
        Assert.That(body, Does.Contain("consult@ehu.lt").Or.Contains("recruitment@ehu.lt"));

        var hasMainLandline = Regex.IsMatch(body, @"\+?\s*370\s*5\s*263\s*9650");
        var hasMobileAdmission = Regex.IsMatch(body, @"\+?\s*370\s*\(?644\)?\s*96\s*317");
        Assert.That(hasMainLandline || hasMobileAdmission, Is.True, "На странице не найден ожидаемый номер телефона (+370 …).");
    }
}
