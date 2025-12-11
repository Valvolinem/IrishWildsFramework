using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests.PageObjects.Components.ControlsBottom
{
    /// <summary>
    /// Spin button component - main button to initiate spin
    /// HTML: button.button__rounded-xl.arrows-spin-button
    /// </summary>
    public class SpinButton
    {
        private readonly IPage _page;
        private readonly string _baseSelector;

        public SpinButton(IPage page, string baseSelector)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _baseSelector = baseSelector ?? throw new ArgumentNullException(nameof(baseSelector));
        }

        /// <summary>
        /// Click the spin button to initiate a spin
        /// </summary>
        public async Task ClickSpin()
        {
            var spinButton = _page.Locator(_baseSelector);
            await spinButton.ClickAsync();
        }

        /// <summary>
        /// Wait for the spinning animation to complete
        /// </summary>
        public async Task WaitForSpinAnimationComplete()
        {
            await _page.WaitForFunctionAsync(
                "() => document.querySelector('.arrows-spin-button animateTransform') === null",
                null,
                new() { Timeout = 50000 }
            );
        }

        /// <summary>
        /// Check if spin button is enabled
        /// </summary>
        public async Task<bool> IsEnabled()
        {
            var button = _page.Locator(_baseSelector);
            return await button.IsEnabledAsync();
        }
    }
}
