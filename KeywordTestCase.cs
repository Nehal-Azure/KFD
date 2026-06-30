namespace TestProject2.Core;


/// <summary>
/// All the steps that share the same TestCaseId in the CSV file, in StepOrder.
/// This is what becomes a single NUnit [Test] at run time, and a single entry
/// in the Allure / Playwright reports.
/// </summary>
public class KeywordTestCase
{
    public string TestCaseId { get; set; } = string.Empty;
    public string TestCaseName { get; set; } = string.Empty;
    public List<TestStep> Steps { get; set; } = new();
 
    // Controls how the test case shows up in the NUnit test explorer, the
    // Playwright HTML report and the Allure report title.
    public override string ToString() => $"{TestCaseId} - {TestCaseName}";
    
}