using Allure.Net.Commons;
using Microsoft.Playwright;
namespace TestProject2.Core;


/// <summary>
/// The actual "engine" of the keyword-driven framework: given one TestStep, look
/// up its keyword in the registry and run it against the current page.
///
/// Each step is wrapped in an Allure report step so the Allure report shows a
/// readable, numbered breakdown of every action a test case performed - not just
/// a single pass/fail line.
/// </summary>
public class KeywordEngine
{
    public async Task ExecuteStepAsync(IPage page, TestStep step)
    {
        var implementation = KeywordRegistry.Resolve(step.Keyword);
        var stepTitle = $"Step {step.StepOrder:00} - {step.Keyword}" +
                        (string.IsNullOrWhiteSpace(step.Description) ? "" : $": {step.Description}");
 
        // AllureApi.Step has both a sync (Action) and an async (Func<Task>) overload
        // in recent Allure.NUnit releases - confirm against the version you install
        // (Allure.NUnit docs: https://allurereport.org/docs/nunit-reference/#test-steps).
        // If your installed version only exposes the sync overload, replace this with
        // a try/catch around `await implementation(page, step);` and call
        // AllureApi.AddAttachment for context manually instead.
        try
        {
            await AllureApi.Step(stepTitle, async () => await implementation(page, step));
        }
        catch (Exception ex)
        {
            throw new KeywordExecutionException(step, ex);
        }
    }
 
    public async Task ExecuteTestCaseAsync(IPage page, KeywordTestCase testCase)
    {
        foreach (var step in testCase.Steps.OrderBy(s => s.StepOrder))
        {
            await ExecuteStepAsync(page, step);
        }
    }
    
}