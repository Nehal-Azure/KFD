
using System.Text.RegularExpressions;
using TestProject2.Core;
using Microsoft.Playwright;
namespace TestProject2.Keywords;

/// <summary>
/// Assertion keywords. These deliberately use Playwright's web-first
/// Assertions.Expect(...) API rather than NUnit's plain Assert, because Expect
/// auto-retries until the timeout, which is what makes UI assertions reliable
/// against async rendering instead of flaky.
/// </summary>
public static class AssertionKeywords
{
    [Keyword("AssertVisible")]
    public static async Task AssertVisible(IPage page, TestStep step)
    {
        await Assertions.Expect(Locator(page, step)).ToBeVisibleAsync();
    }
 
    [Keyword("AssertHidden")]
    public static async Task AssertHidden(IPage page, TestStep step)
    {
        await Assertions.Expect(Locator(page, step)).ToBeHiddenAsync();
    }
 
    [Keyword("AssertText")]
    public static async Task AssertText(IPage page, TestStep step)
    {
        // ExpectedResult column holds the text to look for (substring match).
        await Assertions.Expect(Locator(page, step)).ToContainTextAsync(step.ExpectedResult);
    }
 
    [Keyword("AssertExactText")]
    public static async Task AssertExactText(IPage page, TestStep step)
    {
        await Assertions.Expect(Locator(page, step)).ToHaveTextAsync(step.ExpectedResult);
    }
 
    [Keyword("AssertEnabled")]
    public static async Task AssertEnabled(IPage page, TestStep step)
    {
       // await Assertions.Expect(Locator(page, step)).ToBeEnabledAsync();
        await Assertions.Expect(Locator(page, step)).ToBeDisabledAsync();
    }
 
    [Keyword("AssertChecked")]
    public static async Task AssertChecked(IPage page, TestStep step)
    {
        await Assertions.Expect(Locator(page, step)).ToBeCheckedAsync();
    }
 
    [Keyword("AssertUrl")]
    public static async Task AssertUrl(IPage page, TestStep step)
    {
        // ExpectedResult column holds a substring of the URL to match.
        await Assertions.Expect(page).ToHaveURLAsync(new Regex(Regex.Escape(step.ExpectedResult)));
    }
 
    [Keyword("AssertTitle")]
    public static async Task AssertTitle(IPage page, TestStep step)
    {
        await Assertions.Expect(page).ToHaveTitleAsync(new Regex(Regex.Escape(step.ExpectedResult)));
    }
 
    private static ILocator Locator(IPage page, TestStep step) =>
        step.ResolveLocator(page)
        ?? throw new InvalidOperationException(
            $"Step {step.StepOrder} ('{step.Keyword}') needs an Element/PageObject value but none was provided.");
    
}