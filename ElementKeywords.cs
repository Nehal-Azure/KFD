using TestProject2.Configuration;
using TestProject2.Core;
using Microsoft.Playwright;
namespace TestProject2.Keywords;


/// <summary>
/// Keywords that interact with a single element. The element is resolved via
/// step.ResolveLocator(page), which goes through the Page Object Model when a
/// PageObject column is supplied, or treats Element as a raw selector otherwise.
/// </summary>
public static class ElementKeywords
{
    [Keyword("Click")]
    public static async Task Click(IPage page, TestStep step)
    {
        await Locator(page, step).ClickAsync();
    }
 
    [Keyword("DoubleClick")]
    public static async Task DoubleClick(IPage page, TestStep step)
    {
        await Locator(page, step).DblClickAsync();
    }
 
    [Keyword("EnterText")]
    public static async Task EnterText(IPage page, TestStep step)
    {
        await Locator(page, step).FillAsync(step.Value);
    }
 
    [Keyword("TypeText")]
    public static async Task TypeText(IPage page, TestStep step)
    {
        // Use this instead of EnterText when the app reacts to individual
        // keystrokes (live search, masked inputs, JS keyup handlers).
        await Locator(page, step).PressSequentiallyAsync(step.Value);
    }
 
    [Keyword("SelectOption")]
    public static async Task SelectOption(IPage page, TestStep step)
    {
        await Locator(page, step).SelectOptionAsync(step.Value);
    }
 
    [Keyword("Check")]
    public static async Task Check(IPage page, TestStep step)
    {
        await Locator(page, step).CheckAsync();
    }
 
    [Keyword("Uncheck")]
    public static async Task Uncheck(IPage page, TestStep step)
    {
        await Locator(page, step).UncheckAsync();
    }
 
    [Keyword("Hover")]
    public static async Task Hover(IPage page, TestStep step)
    {
        await Locator(page, step).HoverAsync();
    }
 
    [Keyword("WaitForVisible")]
    public static async Task WaitForVisible(IPage page, TestStep step)
    {
        await Locator(page, step).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });
    }
 
    [Keyword("ScrollIntoView")]
    public static async Task ScrollIntoView(IPage page, TestStep step)
    {
        await Locator(page, step).ScrollIntoViewIfNeededAsync();
    }
 
    private static ILocator Locator(IPage page, TestStep step) =>
        step.ResolveLocator(page)
        ?? throw new InvalidOperationException(
            $"Step {step.StepOrder} ('{step.Keyword}') needs an Element/PageObject value but none was provided.");
    
}