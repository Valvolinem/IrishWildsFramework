using Microsoft.Playwright;
using PlaywrightTests.PageObjects.Components.Displays;

namespace PlaywrightTests.PageObjects.Components.ControlsTop
{
    public class ControlsTop
    {
        private readonly IPage _page;
        private readonly string _baseSelector = ".controls__wrapper._top";

        public BalanceDisplay BalanceDisplay { get; }
        public WinDisplay WinDisplay { get; }

        public ControlsTop(IPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            BalanceDisplay = new BalanceDisplay(page, $"{_baseSelector} .display.balance");
            WinDisplay = new WinDisplay(page, $"{_baseSelector} .display.win");
        }

        public async Task<bool> IsVisible()
        {
            var element = _page.Locator(_baseSelector);
            return await element.IsVisibleAsync();
        }
    }
}
