using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests.PageObjects.Components.ControlsBottom
{
    public class FeatureButtons
    {
        private readonly IPage _page;
        private readonly string _baseSelector;
        
        public FeatureButtons(IPage page, string baseSelector)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _baseSelector = baseSelector ?? throw new ArgumentNullException(nameof(baseSelector));
        }

        public async Task ClickFeatureSpinButton()
        {
            var button = _page.Locator($"{_baseSelector} ._feature-spin-button");
            await button.ClickAsync();
        }

        public async Task ClickBuyFeatureButton()
        {
            var button = _page.Locator($"{_baseSelector} ._buy-feature-button");
            await button.ClickAsync();
        }

        public async Task ClickBuy()
        {
            var button = _page.Locator(".buy__feature button.button__primary");
            await button.ClickAsync();
        }

        public async Task ClickBannerStartBtn()
        {
            var button = _page.Locator(".banner__container button:has-text('START')");
            await button.ClickAsync();
        }

        public async Task WaitForBanner(string? expectedH1Text = null, string? expectedH2Text = null, bool checkButton = false, int timeoutMs = 80000)
        {
            await _page.WaitForSelectorAsync(".banner__container", new PageWaitForSelectorOptions { Timeout = timeoutMs });
            
            if (!string.IsNullOrEmpty(expectedH1Text))
            {
                var h1 = _page.Locator(".banner__container h1");
                var h1Text = await h1.TextContentAsync();
                Assert.IsNotNull(h1Text, "Banner h1 should not be null");
                Assert.IsTrue(h1Text!.Equals(expectedH1Text), $"Banner h1 should equal '{expectedH1Text}' but was: {h1Text}");
            }
            
            if (!string.IsNullOrEmpty(expectedH2Text))
            {
                var h2 = _page.Locator(".banner__container h2");
                var h2Text = await h2.TextContentAsync();
                Assert.IsNotNull(h2Text, "Banner h2 should not be null");
                Assert.IsTrue(h2Text!.Equals(expectedH2Text), $"Banner h2 should equal '{expectedH2Text}' but was: {h2Text}");
            }
            
            if (checkButton)
            {
                var button = _page.Locator(".banner__container button:not(:disabled)");
                var isButtonVisible = await button.IsVisibleAsync();
                Assert.IsTrue(isButtonVisible, "Banner should have an active button");
            }
        }

        public async Task FreeGamesIndicatorIsVisible()
        {
            var freeGamesElement = _page.Locator(".controls__wrapper-free-games");
            var isVisible = await freeGamesElement.IsVisibleAsync();
            Assert.IsTrue(isVisible, "Free games element should be visible");
        }

        public async Task WaitForBannerToDisappear(int timeoutMs = 15000)
        {
            await _page.WaitForSelectorAsync(".banner__container", new PageWaitForSelectorOptions { State = WaitForSelectorState.Hidden, Timeout = timeoutMs });
        }

        public async Task VerifyBuyFeatureLabel(string expectedLabel)
        {
            var firstP = await _page.Locator($"{_baseSelector} .text__wrapper p").Nth(0).TextContentAsync() ?? string.Empty;
            var secondP = await _page.Locator($"{_baseSelector} .text__wrapper p").Nth(1).TextContentAsync() ?? string.Empty;
            var actualLabel = (firstP.Trim() + " " + secondP.Trim()).Trim();
            Assert.AreEqual(expectedLabel.Trim(), actualLabel, 
                $"Expected label '{expectedLabel}', but got '{actualLabel}'");
        }
    }
}
