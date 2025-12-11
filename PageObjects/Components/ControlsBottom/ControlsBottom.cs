using Microsoft.Playwright;

namespace PlaywrightTests.PageObjects.Components.ControlsBottom
{
    public class ControlsBottom
    {
        private readonly IPage _page;
        private readonly string _baseSelector = ".controls__wrapper._bottom";

        public Quantity Quantity { get; }
        public SpinButton SpinButton { get; }
        public FeatureButtons FeatureButtons { get; }

        public ControlsBottom(IPage page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
            Quantity = new Quantity(page, $"{_baseSelector} .quantity");
            SpinButton = new SpinButton(page, $"{_baseSelector} .button__rounded-xl");
            FeatureButtons = new FeatureButtons(page, _baseSelector);
        }

        public async Task<bool> IsVisible()
        {
            var element = _page.Locator(_baseSelector);
            return await element.IsVisibleAsync();
        }

        public async Task ClickAutoplayButton()
        {
            var button = _page.Locator(".controls__autoplay button");
            await button.ClickAsync();
        }

        public async Task ClickPlusStakeAuto(int times = 1)
        {
            var button = _page.Locator(".autoplay__wrapper:has(.autoplay__title:has-text('Stake')) .quantity:not(.quantity-dummy) button.button__stake:has(svg use[*|href='#PLUS'])");
            for (int i = 0; i < times; i++)
            {
                await button.ClickAsync();
            }
        }
    }
}
