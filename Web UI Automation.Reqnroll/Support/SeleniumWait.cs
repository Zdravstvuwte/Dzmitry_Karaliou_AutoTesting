namespace WebUIAutomation.Bdd.Support;

/// <summary>
/// Small polling helpers reused by BDD steps (mirrors waits used in the classic Selenium tests).
/// </summary>
internal static class SeleniumWait
{
    public static bool Until(Func<bool> condition, TimeSpan timeout)
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
                // transient DOM/network instability
            }

            Thread.Sleep(200);
        }

        return false;
    }
}
