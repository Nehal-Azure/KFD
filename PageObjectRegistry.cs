using Microsoft.Playwright;
namespace TestProject2.Pages;

 
/// <summary>
/// Maps a CSV "PageObject" value (e.g. "LoginPage") to the concrete page object
/// class. This is the single place that wires the keyword-driven CSV layer to
/// the Page Object Model layer.
///
/// Adding a new page: write the page object class, then add one line here.
/// </summary>
public static class PageObjectRegistry
{
    private static readonly Dictionary<string, Func<IPage, BasePage>> Factories = new()
    {
        ["LoginPage"] = page => new LoginPage(page),
        ["DashboardPage"] = page => new DashboardPage(page),
    };
 
    public static ILocator Resolve(IPage page, string pageObjectName, string elementName)
    {
        if (!Factories.TryGetValue(pageObjectName, out var factory))
        {
            throw new KeyNotFoundException(
                $"Page object '{pageObjectName}' is not registered in PageObjectRegistry. " +
                $"Known page objects: {string.Join(", ", Factories.Keys)}");
        }
 
        var pageObject = factory(page);
        return pageObject.GetLocator(elementName);
    }
    
}