using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Configuration;
using PlaywrightTests.Helpers;
using PlaywrightTests.Services;
using System.Threading.Tasks;

namespace PlaywrightTests.Tests
{
    public class BasePlaywrightTest
    {
        private static IServiceProvider? _serviceProvider;
        protected IBrowserService BrowserService = default!;
        
        protected IBrowser Browser = default!;
        protected IBrowserContext Context = default!;
        protected IPage Page = default!;
        protected TestConfig TestConfig = default!;

        /// <summary>
        /// Initializes DI container and configuration
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Initialize DI container once for all tests
            if (_serviceProvider == null)
            {
                _serviceProvider = ServiceConfiguration.ConfigureServices();
            }
            
            // Get configuration from DI container
            TestConfig = _serviceProvider.GetService(typeof(TestConfig)) as TestConfig ?? new TestConfig();
        }

        /// <summary>
        /// Each test gets a fresh page and context
        /// </summary>
        [SetUp]
        public async Task SetUp()
        {
            Console.WriteLine($"Running: {TestContext.CurrentContext.Test.Name}");
            
            // Get browser service from DI container
            BrowserService = (_serviceProvider?.GetService(typeof(IBrowserService)) as IBrowserService) 
                ?? throw new InvalidOperationException("Browser service not configured");

            // Get browser from pool with retry
            await RetryPolicies.ExecuteWithRetryAsync(
                async () =>
                {
                    Browser = await BrowserService.GetBrowserAsync(TestConfig);
                    
                    // Create FRESH context for each test to ensure isolation
                    var contextOptions = new BrowserNewContextOptions
                    {
                        IgnoreHTTPSErrors = true,
                    };
                    
                    // Add device emulation if configured (e.g., "iPhone 12", "iPhone 13", "Pixel 5")
                    if (!string.IsNullOrEmpty(TestConfig.Device))
                    {
                        var playwright = await Playwright.CreateAsync();
                        
                        // Find device with case-insensitive lookup
                        var deviceKey = playwright.Devices.Keys.FirstOrDefault(
                            k => k.Equals(TestConfig.Device, StringComparison.OrdinalIgnoreCase)
                        );
                        
                        if (deviceKey != null)
                        {
                            var device = playwright.Devices[deviceKey];
                            contextOptions.ViewportSize = device.ViewportSize;
                            contextOptions.UserAgent = device.UserAgent;
                            contextOptions.DeviceScaleFactor = device.DeviceScaleFactor;
                            contextOptions.IsMobile = device.IsMobile;
                            contextOptions.HasTouch = device.HasTouch;
                            Console.WriteLine($"Device emulation enabled: {deviceKey}");
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Device '{TestConfig.Device}' not found. Available devices: {string.Join(", ", playwright.Devices.Keys)}");
                        }
                    }
                    
                    Context = await Browser.NewContextAsync(contextOptions);
                    
                    // Create fresh page in the isolated context
                    Page = await Context.NewPageAsync();
                    Page.SetDefaultTimeout(TestConfig.ActionTimeout);
                    Page.SetDefaultNavigationTimeout(TestConfig.NavigationTimeout);
                },
                "Browser and context initialization"
            );
        }

        protected async Task Navigate(string url)
        {
            await Page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
        }

        /// <summary>
        /// Closes context and clears all test-specific state
        /// </summary>
        [TearDown]
        public async Task TearDown()
        {
            var testOutcome = TestContext.CurrentContext.Result.Outcome;
            var testName = TestContext.CurrentContext.Test.Name;
            var isPassed = testOutcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
            
            var green = "\x1b[32m";
            var red = "\x1b[31m";
            var reset = "\x1b[0m";
            
            var statusText = isPassed ? "PASSED" : "FAILED";
            var color = isPassed ? green : red;
            
            Console.WriteLine($"{color}{statusText}: {testName}{reset}");
            
            try
            {
                // Take screenshot on test failure
                if (testOutcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    try
                    {
                        await PlaywrightHelpers.TakeScreenshot(Page, TestContext.CurrentContext.Test.Name);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to capture screenshot: {ex.Message}");
                    }
                }

                if (Context != null)
                {
                    try
                    {
                        var pages = Context.Pages.ToList();
                        foreach (var page in pages)
                        {
                            try
                            {
                                if (!page.IsClosed)
                                {
                                    await page.CloseAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to close page: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to get pages: {ex.Message}");
                    }
                }

                // Close the context to ensure complete isolation between tests
                if (Context != null)
                {
                    await Context.CloseAsync();
                }
                
                // Return browser to pool for reuse by other tests
                if (Browser != null && BrowserService != null)
                {
                    await BrowserService.ReturnBrowserAsync(Browser);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Cleanup fixture - runs once after all tests
        /// </summary>
        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            // Dispose browser service if it's disposable
            if (BrowserService is IAsyncDisposable disposable)
            {
                await disposable.DisposeAsync();
            }            
        }
    }
}
