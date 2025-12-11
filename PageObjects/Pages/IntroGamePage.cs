using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Constants;
using PlaywrightTests.Helpers;

namespace PlaywrightTests.PageObjects.Pages
{
    public class IntroGamePage
    {
        private readonly IPage _page;
        private readonly ILocator _progressBar;

        public ILocator PlayButton { get; }

        public IntroGamePage(IPage page)
        {
            _page = page;
            _progressBar = page.Locator(AppConstants.Selectors.ProgressBarHidden);
            PlayButton = page.Locator(AppConstants.Selectors.SliderControlsWrapper).Locator(AppConstants.Selectors.SliderPlayButton);
        }

        public async Task WaitForProgressBarComplete(int timeoutMs = 15000)
        {
            await WaitHelpers.WaitForElementHidden(
                _progressBar,
                timeoutMs: timeoutMs,
                description: "Progress bar"
            );
        }

        public async Task AssertPlayButtonVisibleAndClickable()
        {
            await PlaywrightHelpers.WaitForElementVisibleAndClickable(PlayButton);
        }

        public async Task VerifyErrorMessage(string expectedErrorMessage)
        {
            var errorTitle = _page.Locator(".error__title");
            var actualMessage = await errorTitle.TextContentAsync() ?? string.Empty;
            Assert.AreEqual(expectedErrorMessage.Trim(), actualMessage.Trim(),
                $"Expected error message '{expectedErrorMessage}', but got '{actualMessage}'");
        }
    }
}
