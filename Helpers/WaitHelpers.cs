using Microsoft.Playwright;
using System.Diagnostics;

namespace PlaywrightTests.Helpers
{
    public static class WaitHelpers
    {
        /// <summary>
        /// Wait for element to appear and be stable (no further DOM changes)
        /// </summary>
        public static async Task WaitForElementStable(
            ILocator locator,
            int stabilityDelayMs = 500,
            int timeoutMs = 10000)
        {
            var stopwatch = Stopwatch.StartNew();
            var timespan = TimeSpan.FromMilliseconds(timeoutMs);
            int stableCount = 0;
            const int requiredStableChecks = 2;

            while (stopwatch.Elapsed < timespan)
            {
                try
                {
                    var isVisible = await locator.IsVisibleAsync();
                    if (isVisible)
                    {
                        stableCount++;
                        if (stableCount >= requiredStableChecks)
                        {
                            return;
                        }
                        await Task.Delay(stabilityDelayMs);
                    }
                    else
                    {
                        stableCount = 0;
                    }
                }
                catch
                {
                    stableCount = 0;
                    await Task.Delay(100);
                }
            }

            throw new TimeoutException($"Element did not stabilize within {timeoutMs}ms");
        }

        /// <summary>
        /// Wait for element to be hidden with logging
        /// </summary>
        public static async Task WaitForElementHidden(
            ILocator locator,
            int timeoutMs = 10000,
            string? description = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var options = new LocatorWaitForOptions { Timeout = timeoutMs, State = WaitForSelectorState.Hidden };
            
            try
            {
                await locator.WaitForAsync(options);
            }
            catch (TimeoutException)
            {
                throw;
            }
        }
    }
}
