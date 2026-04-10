using OpenQA.Selenium;

namespace WebUIAutomation.Tests.Pages;

public sealed class SearchResultsPage(IWebDriver driver) : BasePage(driver)
{
    public bool HasQueryInUrl(string query, TimeSpan timeout)
    {
        return WaitUntil(
            () =>
            {
                var url = Driver.Url;
                if (!url.Contains("?s=", StringComparison.OrdinalIgnoreCase)
                    && !url.Contains("&s=", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var decoded = Uri.UnescapeDataString(url);
                var tokens = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                return tokens.All(token => decoded.Contains(token, StringComparison.OrdinalIgnoreCase));
            },
            timeout);
    }

    public bool ShowsResultsContext(TimeSpan timeout)
    {
        return WaitUntil(
            () =>
            {
                var pageText = Driver.FindElement(By.TagName("body")).Text;
                return pageText.Contains("You searched for", StringComparison.OrdinalIgnoreCase)
                    || pageText.Contains("Search results", StringComparison.OrdinalIgnoreCase)
                    || pageText.Contains("results found", StringComparison.OrdinalIgnoreCase);
            },
            timeout);
    }
}
