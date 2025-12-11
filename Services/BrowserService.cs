using Microsoft.Playwright;
using PlaywrightTests.Configuration;

namespace PlaywrightTests.Services
{
    /// <summary>
    /// Browser service for managing browser pool with proper lifecycle management
    /// </summary>
    public class BrowserService : IBrowserService, IAsyncDisposable
    {
        private readonly List<IBrowser> _availableBrowsers = new();
        private readonly List<IBrowser> _allBrowsers = new();
        private readonly object _lockObject = new();

        /// <summary>
        /// Gets a browser instance from the pool or creates a new one
        /// </summary>
        public async Task<IBrowser> GetBrowserAsync(TestConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            lock (_lockObject)
            {
                if (_availableBrowsers.Count > 0)
                {
                    var browser = _availableBrowsers[0];
                    _availableBrowsers.RemoveAt(0);
                    return browser;
                }
            }

            return await CreateNewBrowserAsync(config);
        }

        /// <summary>
        /// Returns a browser to the pool for reuse
        /// </summary>
        public async Task ReturnBrowserAsync(IBrowser browser)
        {
            if (browser == null)
                return;

            try
            {
                // Only return browser to pool if it's still connected
                if (browser.IsConnected)
                {
                    lock (_lockObject)
                    {
                        _availableBrowsers.Add(browser);
                    }
                }
                else
                {
                    // Browser is closed, discard it
                    await browser.CloseAsync();
                    lock (_lockObject)
                    {
                        _allBrowsers.Remove(browser);
                    }
                }
            }
            catch (Exception)
            {
                try
                {
                    await browser.CloseAsync();
                }
                catch
                {
                    Console.WriteLine("Failed to close browser during error handling");
                }

                lock (_lockObject)
                {
                    _allBrowsers.Remove(browser);
                }
            }
        }

        /// <summary>
        /// Creates a new browser instance
        /// </summary>
        private async Task<IBrowser> CreateNewBrowserAsync(TestConfig config)
        {
            // Create a fresh Playwright instance for each browser
            var playwright = await Playwright.CreateAsync();

            var opts = new BrowserTypeLaunchOptions
            {
                Headless = config.Headless,
                SlowMo = config.SlowMo
            };

            IBrowser browser = config.Browser?.ToLower() switch
            {
                "firefox" => await playwright.Firefox.LaunchAsync(opts),
                "webkit" => await playwright.Webkit.LaunchAsync(opts),
                _ => await playwright.Chromium.LaunchAsync(opts)
            };

            lock (_lockObject)
            {
                _allBrowsers.Add(browser);
            }

            return browser;
        }

        /// <summary>
        /// Disposes all resources
        /// </summary>
        public async Task DisposeAsync()
        {
            List<IBrowser> browsersToDispose;
            lock (_lockObject)
            {
                browsersToDispose = new List<IBrowser>(_allBrowsers);
                _allBrowsers.Clear();
                _availableBrowsers.Clear();
            }

            foreach (var browser in browsersToDispose)
            {
                try
                {
                    await browser.CloseAsync();
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to close browser during disposal");
                }
            }

        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            return new ValueTask(DisposeAsync());
        }
    }
}
