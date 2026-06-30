using Microsoft.Playwright;
namespace TestProject2.Pages;

/// <summary>
/// Base class for every Page Object. A page object's only job is to map a
/// human-readable element name (used in the CSV "Element" column) to the real
/// Playwright selector. This is what lets the CSV say:
///
///   PageObject=LoginPage, Element=UsernameInput
///
/// instead of leaking CSS selectors into test data - if the markup changes, you
/// fix it in one place (this class), and every CSV row that uses
/// "LoginPage.UsernameInput" keeps working unchanged.
/// </summary>
public abstract class BasePage
{
    protected readonly IPage Page;
 
    protected BasePage(IPage page)
    {
        Page = page;
    }
    
    /// <summary>Logical element name -> Playwright selector (CSS, text=, role=, etc.)</summary>
    protected abstract IReadOnlyDictionary<string, string> ElementMap { get; }
 
    public ILocator GetLocator(string elementName)
    {
        if (!ElementMap.TryGetValue(elementName, out var selector))
        {
            throw new KeyNotFoundException(
                $"Element '{elementName}' is not mapped in {GetType().Name}. " +
                $"Known elements: {string.Join(", ", ElementMap.Keys)}");
        }
 
        return Page.Locator(selector);
    }
    
}