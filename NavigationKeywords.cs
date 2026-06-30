using TestProject2.Configuration;
using TestProject2.Core;
using Microsoft.Playwright;
namespace TestProject2.Keywords;

/// <summary>
/// Keywords that act on the page/browser itself rather than a specific element.
/// </summary>
public static class NavigationKeywords
{
    [Keyword("NavigateTo")]
    public static async Task NavigateTo(IPage page, TestStep step)
    {
        var url = step.Value;
 
        // Allow either a full URL or a relative path appended to the configured BaseUrl,
        // so CSV authors can write "/inventory.html" instead of repeating the full domain.
        if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            url = $"{TestSettings.Current.BaseUrl.TrimEnd('/')}/{url.TrimStart('/')}";
        }
 
        await page.GotoAsync(url);
    }
 
    [Keyword("Reload")]
    public static async Task Reload(IPage page, TestStep step)
    {
        await page.ReloadAsync();
    }
 
    [Keyword("GoBack")]
    public static async Task GoBack(IPage page, TestStep step)
    {
        await page.GoBackAsync();
    }
 
    [Keyword("Wait")]
    public static async Task Wait(IPage page, TestStep step)
    {
        // Value column holds milliseconds. Use sparingly - prefer WaitForVisible /
        // Playwright's built-in auto-waiting wherever possible; this exists for the
        // rare case (animations, debounced UI) where an explicit pause is the
        // pragmatic option.
        var ms = int.Parse(step.Value);
        await page.WaitForTimeoutAsync(ms);
    }
    
    
}