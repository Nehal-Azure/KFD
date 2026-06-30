using TestProject2.Pages;

namespace TestProject2.Core;
using Microsoft.Playwright;


/// <summary>
/// One row in the CSV file = one step of a test case.
///
/// CSV columns:
///   TestCaseId, TestCaseName, StepOrder, Keyword, PageObject, Element, Value, ExpectedResult, Description
///
/// PageObject + Element together resolve a locator through the Page Object Model
/// (e.g. PageObject="LoginPage", Element="UsernameInput" -> LoginPage's #user-name).
/// If PageObject is left blank, Element is treated as a raw Playwright selector
/// (CSS, text=, role=, etc.) so power users/non-POM steps still work.
/// </summary>
public class TestStep
{
    public int StepOrder { get; set; }
    public string Keyword { get; set; } = string.Empty;
    public string PageObject { get; set; } = string.Empty;
    public string Element { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ExpectedResult { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Resolves the locator for this step, going through the Page Object Model
    /// when a PageObject is specified, or falling back to a raw selector.
    /// Returns null when the keyword doesn't need a locator at all (e.g. NavigateTo).
    /// </summary>
    public ILocator? ResolveLocator(IPage page)
    {
        if (string.IsNullOrWhiteSpace(Element))
        {
            return null;
        }
 
        return string.IsNullOrWhiteSpace(PageObject)
            ? page.Locator(Element)
            : PageObjectRegistry.Resolve(page, PageObject, Element);
    }
 
    public override string ToString() =>
        $"Step {StepOrder}: {Keyword}({PageObject}.{Element} = '{Value}') - {Description}";
    
}