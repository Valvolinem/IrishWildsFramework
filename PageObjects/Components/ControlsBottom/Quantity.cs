using Microsoft.Playwright;

namespace PlaywrightTests.PageObjects.Components.ControlsBottom
{
    public class Quantity
    {
        private readonly IPage _page;
        private readonly string _baseSelector;

        public Quantity(IPage page, string baseSelector)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _baseSelector = baseSelector ?? throw new ArgumentNullException(nameof(baseSelector));
        }

        public async Task ClickMinusButton()
        {
            var buttons = _page.Locator($"{_baseSelector} .button__stake");
            await buttons.First.ClickAsync();
        }

        public async Task ClickPlusButton()
        {
            var buttons = _page.Locator($"{_baseSelector} .button__stake");
            await buttons.Last.ClickAsync();
        }

        public async Task<decimal> GetStakeAmountDecimal()
        {
            var amountElement = _page.Locator($"{_baseSelector} .display.stake .amount");
            var amountText = await amountElement.TextContentAsync() ?? string.Empty;
            if (decimal.TryParse(amountText.Replace("$", "").Replace(",", ""), out var amount))
            {
                return amount;
            }
            return 0m;
        }
    }
}
