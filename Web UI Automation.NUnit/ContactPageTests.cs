using System.Text.RegularExpressions;
using NUnit.Framework;
using Shouldly;
using WebUIAutomation.Tests.Pages;

namespace WebUIAutomation.Tests;

[TestFixture]
[Category("Smoke")]
public class ContactPageTests : SeleniumTestBase
{
    protected override TimeSpan? PageLoadTimeout => TimeSpan.FromSeconds(60);

    [Test]
    public void ContactsPage_ShouldShowEmailAndPhone_ForUniversityInquiries()
    {
        var contactsPage = new ContactsPage(Driver).Open();
        var loaded = contactsPage.IsLoaded(TimeSpan.FromSeconds(25));
        loaded.ShouldBeTrue("Страница контактов не загрузилась.");

        Driver.Url.ToLowerInvariant().ShouldContain("/contacts");
        var headingOk = contactsPage.HasContactHeader(TimeSpan.FromSeconds(15));
        headingOk.ShouldBeTrue("Ожидался заголовок страницы с «Contact».");
        var body = contactsPage.BodyText();

        body.ShouldContain("office@ehu.lt");
        (body.Contains("consult@ehu.lt", StringComparison.OrdinalIgnoreCase)
            || body.Contains("recruitment@ehu.lt", StringComparison.OrdinalIgnoreCase))
            .ShouldBeTrue("Ожидался контактный email consult@ehu.lt или recruitment@ehu.lt");

        var hasMainLandline = Regex.IsMatch(body, @"\+?\s*370\s*5\s*263\s*9650");
        var hasMobileAdmission = Regex.IsMatch(body, @"\+?\s*370\s*\(?644\)?\s*96\s*317");
        (hasMainLandline || hasMobileAdmission).ShouldBeTrue("На странице не найден ожидаемый номер телефона (+370 …).");
    }
}
