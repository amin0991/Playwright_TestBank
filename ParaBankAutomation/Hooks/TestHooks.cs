using System;
using System.IO;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Playwright;
using Serilog;
using TechTalk.SpecFlow;
using ParaBankAutomation.Utilities;
//using Allure.NUnit;
//using Allure.NUnit.Attributes;


namespace ParaBankAutomation.Hooks
{
    [Binding]
    public class TestHooks
    {
        private readonly IObjectContainer _container;
        private readonly ScenarioContext _scenarioContext;
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;
        private readonly TestConfiguration _config;

        public TestHooks(IObjectContainer container, ScenarioContext scenarioContext)
        {
            _container = container;
            _scenarioContext = scenarioContext;
            _config = new TestConfiguration();

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File($"logs/test-execution-{DateTime.Now:yyyyMMdd}.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            Log.Information($"Starting scenario: {_scenarioContext.ScenarioInfo.Title}");

            _playwright = await Playwright.CreateAsync();

            _browser = _config.Browser.ToLower() switch
            {
                "firefox" => await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = _config.Headless }),
                "webkit" => await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions { Headless = _config.Headless }),
                _ => await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = _config.Headless })
            };

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
            });

            _page = await _context.NewPageAsync();
            _page.SetDefaultTimeout(_config.Timeout);

            _container.RegisterInstanceAs(_page);
            _container.RegisterInstanceAs(_config);
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
            
            if (_scenarioContext.TestError != null)
            {
                Log.Error($"Scenario failed: {scenarioTitle}");
                Log.Error($"Error: {_scenarioContext.TestError.Message}");

                // Take screenshot on failure with custom naming: BELHAJJI_Test_MMddyyyyHHmmss.png
                if (_config.ScreenshotOnFailure && _page != null)
                {
                    try
                    {
                        // Force create screenshots directory if it doesn't exist
                        var screenshotDir = Path.Combine(Directory.GetCurrentDirectory(), "screenshots");
                        Directory.CreateDirectory(screenshotDir);
                        
                        // Custom filename format: BELHAJJI_Test_MMddyyyyHHmmss.png
                        var fileName = $"BELHAJJI_Test_{DateTime.Now:MMddyyyyHHmmss}.png";
                        var screenshotPath = Path.Combine(screenshotDir, fileName);
                        
                        await _page.ScreenshotAsync(new PageScreenshotOptions
                        {
                            Path = screenshotPath,
                            FullPage = true,
                            Timeout = 10000  // 10 second timeout for screenshot
                        });
                        
                        Log.Information($"Screenshot saved: {screenshotPath}");
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"Failed to capture screenshot: {ex.Message}");
                        // Continue test cleanup even if screenshot fails
                    }
                }
            }
            else
            {
                Log.Information($"Scenario passed: {scenarioTitle}");
            }

            // Cleanup resources
            if (_page != null)
            {
                await _page.CloseAsync();
            }

            if (_context != null)
            {
                await _context.CloseAsync();
            }

            if (_browser != null)
            {
                await _browser.CloseAsync();
            }

            if (_playwright != null)
            {
                _playwright.Dispose();
            }
        }
    }
}