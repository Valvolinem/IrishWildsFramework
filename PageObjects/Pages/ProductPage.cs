using Microsoft.Playwright;
using PlaywrightTests.Constants;
using PlaywrightTests.Helpers;
using Serilog;
using System.Threading.Tasks;

namespace PlaywrightTests.PageObjects.Pages
{
    /// <summary>
    /// Product listing page object (Spinberry home page)
    /// Handles interactions with the video game slider and product cards
    /// </summary>
    public class ProductPage
    {
        private readonly IPage _page;
        private readonly ILocator _videoSliderRoot;
        public ProductPage(IPage page)
        {
            _page = page;
            _videoSliderRoot = page.Locator(AppConstants.Selectors.VideoSlider);
        }

        /// <summary>
        /// Find game locator by name
        /// </summary>
        private async Task<ILocator?> FindGameLocatorAsync(string gameName)
        {
            var games = _videoSliderRoot.Locator(AppConstants.Selectors.GameCard);
            var count = await games.CountAsync();

            for (int i = 0; i < count; i++)
            {
                var gameLocator = games.Nth(i);
                var title = await GetGameTitleFromLocatorAsync(gameLocator);
                
                if (title == gameName)
                    return gameLocator;
            }

            return null;
        }

        /// <summary>
        /// Get game title from a game locator
        /// </summary>
        private async Task<string> GetGameTitleFromLocatorAsync(ILocator gameLocator)
        {
            var titleLocator = gameLocator.Locator(AppConstants.Selectors.GameName);
            var title = await titleLocator.TextContentAsync();
            return title?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Check if play button is visible for a game locator
        /// </summary>
        private async Task<bool> IsPlayButtonVisibleOnLocatorAsync(ILocator gameLocator)
        {
            var playButton = gameLocator.Locator(AppConstants.Selectors.PlayButton);
            return await playButton.IsVisibleAsync();
        }

        /// <summary>
        /// Click play button on a game locator
        /// </summary>
        private async Task ClickPlayButtonOnLocatorAsync(ILocator gameLocator)
        {
            var playButton = gameLocator.Locator(AppConstants.Selectors.PlayButton);
            await playButton.ClickAsync();
        }

        /// <summary>
        /// Play game by exact name match
        /// </summary>
        public async Task PlayGameByName(string gameName)
        {
            var gameLocator = await FindGameLocatorAsync(gameName);
            if (gameLocator == null)
                throw new System.Exception($"Game \"{gameName}\" not found in the slider");

            await gameLocator.ScrollIntoViewIfNeededAsync();
            await WaitHelpers.WaitForElementStable(gameLocator, stabilityDelayMs: 200);
            await ClickPlayButtonOnLocatorAsync(gameLocator);
            
        }

        /// <summary>
        /// Get currently visible game name (first visible game)
        /// </summary>
        public async Task<string> GetCurrentGameName()
        {
            var firstGame = _videoSliderRoot.Locator(AppConstants.Selectors.GameCard).First;
            await firstGame.ScrollIntoViewIfNeededAsync();
            
            var gameTitle = firstGame.Locator(AppConstants.Selectors.GameTitle);
            var gameName = await gameTitle.TextContentAsync();
            
            return gameName?.Trim() ?? "Unknown Game";
        }

        /// <summary>
        /// Check if a specific game is playable (has play button)
        /// </summary>
        public async Task<bool> IsGamePlayable(string gameName)
        {
            var gameLocator = await FindGameLocatorAsync(gameName);
            if (gameLocator == null)
                return false;

            return await IsPlayButtonVisibleOnLocatorAsync(gameLocator);
        }
    }
}
