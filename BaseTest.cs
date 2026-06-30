using Allure.Net.Commons;
using Allure.NUnit;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TestProject2.Configuration;
using TestProject2.Core;

namespace TestProject2.Tests;
/// <summary>
/// Shared setup/teardown for every CSV-driven test fixture.
///
/// Inherits from PlaywrightTest (Microsoft.Playwright.NUnit) only for its
/// per-worker IPlaywright driver management - everything else (Browser,
/// Context, Page) is created and torn down explicitly here so we control the
/// exact moment the context closes. That control matters: Playwright only
/// finalizes a recorded video file once its context is closed, so closing it
/// ourselves - after grabbing the trace and the failure screenshot - is what
/// guarantees the video file actually exists before we try to attach it.
///
/// [AllureNUnit] turns on Allure result capture for every test in this class
/// (and in anything that derives from it).
/// </summary>
[AllureNUnit]
public abstract class BaseTest : PlaywrightTest
{
    protected readonly KeywordEngine Engine = new();
 
    protected IBrowser Browser { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;
 
    private static readonly string ArtifactsRoot =
        Path.Combine(AppContext.BaseDirectory, "test-artifacts");
 
    private string _videoDir = string.Empty;
 
    [SetUp]
    public async Task SetUpBrowserAsync()
    {
        
        var settings = TestSettings.Current;
 
        // Printed every run via TestContext.Progress (shows in `dotnet test` console
        // output and the IDE test output pane) so a misconfigured appsettings.json -
        // or a leftover environment variable silently overriding it - is obvious
        // immediately instead of being a guessing game.
        TestContext.Progress.WriteLine(
            $"[TestProject2] Browser={settings.Browser} Channel={settings.Channel ?? "(bundled)"} " +
            $"Headless={settings.Headless} SlowMoMs={settings.SlowMoMs} TraceMode={settings.TraceMode} BaseUrl={settings.BaseUrl}");
       
        
        Browser = await ResolveBrowserType().LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = TestSettings.Current.Headless,
            Channel = string.IsNullOrWhiteSpace(settings.Channel) ? null : settings.Channel,
            SlowMo = settings.SlowMoMs
        });
 
        // One video sub-folder per test so parallel workers never collide on the
        // same .webm file, and so we can find "the" video for this test afterwards.
        _videoDir = Path.Combine(ArtifactsRoot, "videos", TestContext.CurrentContext.Test.ID);
        Directory.CreateDirectory(_videoDir);
 
        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = _videoDir,
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        });
 
        Context.SetDefaultTimeout(TestSettings.Current.DefaultTimeoutMs);
 
        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Title = TestContext.CurrentContext.Test.Name,
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
 
        Page = await Context.NewPageAsync();
    }
 
    [TearDown]
    public async Task TearDownBrowserAsync()
    {
        var settings = TestSettings.Current;
        var result = TestContext.CurrentContext.Result;
        var failed = result.Outcome.Status == TestStatus.Failed;
        var passed = result.Outcome.Status == TestStatus.Passed;
        var safeName = Sanitize(TestContext.CurrentContext.Test.Name);
 
        //Not used yet
        var shouldSaveTrace = settings.TraceMode switch
        {
            "Always" => true,
            "Off" => false,
            _ => failed // "OnFailure" (default) and any unrecognized value - the safe choice
        };
        
        if (failed)
        {
            // Best-effort: the page may already be in a broken state, don't let a
            // screenshot failure hide the real assertion failure.
            try
            {
                var screenshot = await Page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
                AllureApi.AddAttachment($"{safeName}-screenshot.png", "image/png", screenshot);
            }
            catch
            {
                // Intentionally swallowed - reporting must never mask the original failure.
            }
        }
 
        var traceDir = Path.Combine(ArtifactsRoot, "traces");
        Directory.CreateDirectory(traceDir);
        var tracePath = Path.Combine(traceDir, $"{safeName}.zip");
 
        // Only persist the trace to disk on failure - tracing.stop() with a Path
        // still has to run for passing tests too, just without saving anywhere,
        // so the recorder is properly flushed before the context closes.
        await Context.Tracing.StopAsync(new TracingStopOptions
        {
           Path = failed ? tracePath : null
           //Path = passed ? tracePath : null
        });
 
        // Close explicitly (and in this order) so the video file is finalized
        // before we try to read it below.
        await Context.CloseAsync();
        await Browser.CloseAsync();
 
        if (failed)
        {
            if (File.Exists(tracePath))
            {
                AllureApi.AddAttachment(
                    $"{safeName}-trace.zip (open at https://trace.playwright.dev)",
                    "application/zip",
                    tracePath);
            }
 
            var videoFile = Directory.Exists(_videoDir)
                ? Directory.GetFiles(_videoDir, "*.webm").FirstOrDefault()
                : null;
 
            if (videoFile is not null)
            {
                AllureApi.AddAttachment($"{safeName}-video.webm", "video/webm", videoFile);
            }
        }
        else
        {
            // Keep disk usage sane: delete passing-test videos, keep only failures.
            try { Directory.Delete(_videoDir, recursive: true); } catch { /* best effort */ }
        }
    }
 
    private IBrowserType ResolveBrowserType() => TestSettings.Current.Browser.ToLowerInvariant() switch
    {
        "firefox" => Playwright.Firefox,
        "webkit" => Playwright.Webkit,
        _ => Playwright.Chromium
    };
 
    private static string Sanitize(string name) =>
        string.Concat(name.Select(c => char.IsLetterOrDigit(c) || c is '-' or '_' ? c : '_'));
    
}