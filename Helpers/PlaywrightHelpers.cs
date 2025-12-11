using Microsoft.Playwright;

namespace PlaywrightTests.Helpers
{
    public enum GameType
    {
        IrishWilds94,
        IcyFruits10_95,
        AllAboutTheWilds95
    }

    public static class PlaywrightHelpers
    {
        public static string GetGameUrl(GameType gameName, string culture = "EN")
        {
            var gameNameStr = gameName.ToString();
            var randomToken = "DEMO_PP_" + Guid.NewGuid().ToString();
            var baseUrl = $"https://d21j22mhfwmuah.cloudfront.net/0Debug/SB_HTML5_{gameNameStr}/index.html?gameCode=SB_HTML5_{gameNameStr}&token=DEMO_PP_63fd6a4d-a8cf-418f-b055-bf7bb60afd52&homeUrl=spinberrysite&rgsUrl=https://rgstorgs.stage.pariplaygames.com&lang={culture}&DebugMode=False&currencyCode=USD&disableRotation=False&ExtraData=networkId%3dPP&HideCoins=True&CoinsDefault=True";
            
            var updatedUrl = System.Text.RegularExpressions.Regex.Replace(
                baseUrl,
                @"token=DEMO_PP_[a-f0-9\-]+",
                $"token={randomToken}"
            );
            
            return updatedUrl;
        }

        public static async Task<(IRequest request, string body)> WaitForRequest(IPage page, string urlPattern, int timeoutMs = 10000)
        {
            var request = await page.WaitForRequestAsync(req => req.Url.Contains(urlPattern), new PageWaitForRequestOptions { Timeout = timeoutMs });
            var postData = request.PostDataBuffer;
            var body = postData != null ? System.Text.Encoding.UTF8.GetString(postData) : string.Empty;
            return (request, body);
        }

        public static async Task WaitForElement(IPage page, string selector, int timeoutMs = 80000)
        {
            await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { Timeout = timeoutMs });
        }

        public static async Task WaitForElementVisibleAndClickable(ILocator locator)
        {
            await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached });
        }

        public static async Task TakeScreenshot(IPage page, string testName)
        {
            try
            {
                var screenshotsDir = Path.Combine(AppContext.BaseDirectory, "screenshots", "failures");
                Directory.CreateDirectory(screenshotsDir);
                
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                var filename = $"{testName}_{timestamp}.png";
                var filepath = Path.Combine(screenshotsDir, filename);
                
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = filepath });
            }
            catch (Exception ex)
            {
                // Silently handle screenshot errors to not interfere with test failure reporting
                System.Console.WriteLine($"Failed to take screenshot: {ex.Message}");
            }
        }
    }
}
