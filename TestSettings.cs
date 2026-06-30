namespace TestProject2.Configuration;
using System.Text.Json;

/// <summary>
/// Central place for environment configuration (which browser, headless or not,
/// base URL, timeouts). Reads appsettings.json once and caches the result.
///
/// Any value can be overridden at run time with an environment variable of the
/// same name, e.g.  BaseUrl=https://staging.example.com dotnet test
/// This is what you point at a different environment from a CI pipeline without
/// touching the file.
/// </summary>
public class TestSettings
{
    
    public string Browser { get; set; } = "chromium";
    public bool Headless { get; set; } = false;
    public string BaseUrl { get; set; } = "https://www.saucedemo.com";
    public int DefaultTimeoutMs { get; set; } = 30000;
    
    /// <summary>
    /// Optional Playwright "channel": "chrome", "msedge", "chrome-beta", etc.
    /// Leave null/empty to use Playwright's own bundled Chromium build instead
    /// of a real, locally-installed browser.
    /// </summary>
    public string? Channel { get; set; }
    
    /// <summary>
    /// Milliseconds Playwright pauses before/after every action (click, type, ...).
    /// 0 = full speed. Useful while watching a headed run; never leave this > 0
    /// for CI, it just makes every test slower for no benefit.
    /// </summary>
    public int SlowMoMs { get; set; }
    
    
    /// <summary>
    /// "Off" | "OnFailure" | "Always". Controls when a Playwright trace .zip is
    /// written to disk and attached to the Allure report.
    ///   OnFailure (default) - matches Playwright's own sane default: traces only
    ///                         cost disk/time for runs you actually need to debug.
    ///   Always              - capture a trace for every test, pass or fail. Useful
    ///                         while you're first wiring up reporting and want to
    ///                         see the attachment appear without forcing a failure.
    ///   Off                 - never capture a trace at all.
    /// </summary>
    public string TraceMode { get; set; } = "OnFailure";
    
    private static TestSettings? _current;
    public static TestSettings Current => _current ??= Load();
    private static TestSettings Load()
    {
        var settings = new TestSettings();
 
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            settings = JsonSerializer.Deserialize<TestSettings>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? settings;
        }
 
        ApplyEnvironmentOverrides(settings);
        return settings;
    }
    
    private static void ApplyEnvironmentOverrides(TestSettings settings)
    {
        var browser = Environment.GetEnvironmentVariable("Browser");
        if (!string.IsNullOrWhiteSpace(browser)) settings.Browser = browser;
 
        var headless = Environment.GetEnvironmentVariable("Headless");
        if (bool.TryParse(headless, out var headlessValue)) settings.Headless = headlessValue;
 
        var baseUrl = Environment.GetEnvironmentVariable("BaseUrl");
        if (!string.IsNullOrWhiteSpace(baseUrl)) settings.BaseUrl = baseUrl;
        
        var channel = Environment.GetEnvironmentVariable("Channel");
        if (!string.IsNullOrWhiteSpace(channel)) settings.Channel = channel;
 
        var slowMo = Environment.GetEnvironmentVariable("SlowMoMs");
        if (int.TryParse(slowMo, out var slowMoValue)) settings.SlowMoMs = slowMoValue;
 
        var traceMode = Environment.GetEnvironmentVariable("TraceMode");
        if (!string.IsNullOrWhiteSpace(traceMode)) settings.TraceMode = traceMode;
    }
    
}