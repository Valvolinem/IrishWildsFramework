using Microsoft.Playwright;
using PlaywrightTests.PageObjects.Components.ControlsBottom;
using PlaywrightTests.PageObjects.Components.ControlsTop;

namespace PlaywrightTests.PageObjects.Pages
{
    public class GamePage
    {
        private readonly IPage _page;

        public ControlsTop ControlsTop { get; }
        public ControlsBottom ControlsBottom { get; }

        public GamePage(IPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            ControlsTop = new ControlsTop(page);
            ControlsBottom = new ControlsBottom(page);
        }

        public async Task WaitForGameLoad(int timeoutMs = 30000)
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task<bool> IsGameReady()
        {
            var controlsTopVisible = await ControlsTop.IsVisible();
            var controlsBottomVisible = await ControlsBottom.IsVisible();
            return controlsTopVisible && controlsBottomVisible;
        }

        public async Task<decimal> GetBalance()
        {
            return await ControlsTop.BalanceDisplay.GetAmountDecimal();
        }

        public async Task VerifyDisplayLabel(string expectedLabel)
        {
            await ControlsTop.BalanceDisplay.VerifyDisplayLabel(expectedLabel);
        }

        public async Task<decimal> GetWin()
        {
            return await ControlsTop.WinDisplay.GetAmountDecimal();
        }

        public async Task<decimal> GetStake()
        {
            return await ControlsBottom.Quantity.GetStakeAmountDecimal();
        }

        public async Task PerformSpin()
        {
            if (!await ControlsBottom.SpinButton.IsEnabled())
                return;

            await ControlsBottom.SpinButton.ClickSpin();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task IncreaseStake(int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                await ControlsBottom.Quantity.ClickPlusButton();
            }
        }

        public async Task DecreaseStake(int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                await ControlsBottom.Quantity.ClickMinusButton();
            }
        }

        public async Task ClickAutoplay()
        {
            await ControlsBottom.ClickAutoplayButton();
        }

        public async Task ClickAutoplayStartButton()
        {
            var button = _page.Locator(".autoplay button:has-text('START')");
            await button.ClickAsync();
        }

        public async Task ClickPlusStakeAuto(int times = 1)
        {
            await ControlsBottom.ClickPlusStakeAuto(times);
        }

        public async Task ClickFeatureSpin()
        {
            await ControlsBottom.FeatureButtons.ClickFeatureSpinButton();
        }

        public async Task ClickBuyFeature()
        {
            await ControlsBottom.FeatureButtons.ClickBuyFeatureButton();
        }

        public async Task WaitForSpinAnimationComplete()
        {
            await ControlsBottom.SpinButton.WaitForSpinAnimationComplete();
        }
        
    }
}
