using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using TestProject2.Core;

namespace TestProject2.Tests;

/// <summary>
/// This is the only "test code" in the framework - and it stays this short
/// regardless of how many scenarios TestData/LoginTests.csv contains. Every
/// TestCaseId in the CSV produces one independent, separately reportable
/// [Test] thanks to [TestCaseSource].
/// </summary>
[TestFixture]
[AllureSuite("Login")]
public class CsvDrivenTests : BaseTest
{
    [Test]
    [TestCaseSource(typeof(CsvTestCaseReader), nameof(CsvTestCaseReader.GetTestCases))]
    public async Task Run(KeywordTestCase testCase)
    {
        AllureApi.AddTestParameter("TestCaseId", testCase.TestCaseId);
        AllureApi.AddTestParameter("Steps", testCase.Steps.Count.ToString());
 
        await Engine.ExecuteTestCaseAsync(Page, testCase);
    }
    
}