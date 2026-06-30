using CsvHelper;
using CsvHelper.Configuration;
using NUnit.Framework;
using System.Globalization;

namespace TestProject2.Core;



/// <summary>
/// Reads a CSV data file and turns it into NUnit TestCaseData, one per distinct
/// TestCaseId. NUnit's [TestCaseSource] calls GetTestCases() at discovery time,
/// so every row in the CSV becomes a visible, individually-runnable, individually
/// reportable test - without writing a single line of test code per scenario.
/// </summary>
public class CsvTestCaseReader
{
    private static readonly string DefaultCsvPath =
        Path.Combine(AppContext.BaseDirectory, "TestData", "LoginTests.csv");
 
    /// <summary>Default source used by [TestCaseSource(typeof(CsvTestCaseReader), nameof(GetTestCases))]</summary>
    public static IEnumerable<TestCaseData> GetTestCases() => GetTestCasesFromCsv(DefaultCsvPath);
 
    // IMPORTANT: this must NOT also be named GetTestCases. NUnit's TestCaseSourceAttribute
    // resolves its source by name via Type.GetMethod("GetTestCases"), and .NET reflection
    // throws AmbiguousMatchException the moment a type has two methods sharing that name -
    // regardless of differing parameters. NUnit swallows that during discovery and reports
    // the generic "The test case source could not be found" with no stack trace, and only
    // one (failing) placeholder test instead of one test per CSV row.
    public static IEnumerable<TestCaseData> GetTestCasesFromCsv(string csvPath)
    {
        foreach (var testCase in ReadTestCases(csvPath))
        {
            // SetName controls the label shown in the NUnit test tree, the
            // Playwright HTML report and the Allure report - keep it readable.
            yield return new TestCaseData(testCase)
                .SetName($"{testCase.TestCaseId}_{Sanitize(testCase.TestCaseName)}");
        }
    }
 
    public static IEnumerable<KeywordTestCase> ReadTestCases(string csvPath)
    {
        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null
        });
 
        var rows = csv.GetRecords<CsvRow>().ToList();
 
        return rows
            .GroupBy(r => r.TestCaseId)
            .Select(group => new KeywordTestCase
            {
                TestCaseId = group.Key,
                TestCaseName = group.First().TestCaseName,
                Steps = group
                    .OrderBy(r => r.StepOrder)
                    .Select(r => new TestStep
                    {
                        StepOrder = r.StepOrder,
                        Keyword = r.Keyword,
                        PageObject = r.PageObject,
                        Element = r.Element,
                        Value = r.Value,
                        ExpectedResult = r.ExpectedResult,
                        Description = r.Description
                    })
                    .ToList()
            })
            .ToList();
    }
 
    private static string Sanitize(string name) =>
        string.Concat(name.Select(c => char.IsLetterOrDigit(c) ? c : '_'));
 
    // One-to-one mapping of CSV columns. Keep this in sync with the header row
    // of every CSV file in TestData/.
    private sealed class CsvRow
    {
        public string TestCaseId { get; set; } = string.Empty;
        public string TestCaseName { get; set; } = string.Empty;
        public int StepOrder { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public string PageObject { get; set; } = string.Empty;
        public string Element { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string ExpectedResult { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    
}