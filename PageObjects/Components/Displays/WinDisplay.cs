using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests.PageObjects.Components.Displays
{
    public class WinDisplay
    {
        private readonly IPage _page;
        private readonly string _baseSelector;

        public WinDisplay(IPage page, string baseSelector)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            _baseSelector = baseSelector ?? throw new ArgumentNullException(nameof(baseSelector));
        }

        public async Task<decimal> GetAmountDecimal()
        {
            var amountElement = _page.Locator($"{_baseSelector} .amount");
            var amountText = await amountElement.TextContentAsync() ?? string.Empty;
            if (decimal.TryParse(amountText.Replace("$", "").Replace(",", ""), out var amount))
            {
                return amount;
            }
            return 0m;
        }

        public async Task VerifyDisplayLabel(string expectedLabel)
        {
            var actualLabel = await _page.Locator($"{_baseSelector} span").First.TextContentAsync() ?? string.Empty;
            Assert.AreEqual(expectedLabel.Trim(), actualLabel.Trim(), 
                $"Expected label '{expectedLabel}', but got '{actualLabel}'");
        }
    }
}
