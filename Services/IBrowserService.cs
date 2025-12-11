using Microsoft.Playwright;
using PlaywrightTests.Configuration;

namespace PlaywrightTests.Services
{
    /// <summary>
    /// Service interface for managing browser instances and lifecycle
    /// </summary>
    public interface IBrowserService
    {
        /// <summary>
        /// Gets a browser instance from the pool or creates a new one
        /// </summary>
        Task<IBrowser> GetBrowserAsync(TestConfig testConfig);

        /// <summary>
        /// Returns a browser to the pool for reuse
        /// </summary>
        Task ReturnBrowserAsync(IBrowser browser);

        /// <summary>
        /// Disposes all resources
        /// </summary>
        Task DisposeAsync();
    }
}
