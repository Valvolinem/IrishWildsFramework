using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.PageObjects.Pages;
using PlaywrightTests.Helpers;
using PlaywrightTests.Constants;

namespace PlaywrightTests.Tests
{
    [TestFixture]
    public class IrishWildsTests : BasePlaywrightTest
    {
        /// <summary>
        /// Test: Perform spin with stake 10$ and verify balance and win amount after a losing spin
        /// </summary>
        [Test]
        [Retry(3)]
        [Category("Game")]
        public async Task VerifyBalanceAndWinAmountAfterLoss()
        {
            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94));

            var introPage = new IntroGamePage(Page);
            await introPage.WaitForProgressBarComplete();
            await introPage.AssertPlayButtonVisibleAndClickable();
            await introPage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var game = new GamePage(Page);
            await game.WaitForGameLoad();

            await game.ControlsBottom.IsVisible();
            await game.IncreaseStake(6);
            await game.PerformSpin();
            var balBefore = await game.GetBalance();
            await game.WaitForSpinAnimationComplete();
            var win = await game.GetWin();
            Assert.AreEqual(0.0m, win);
            var balAfter = await game.GetBalance();
            Assert.AreEqual(balBefore, balAfter);
        }

        /// <summary>
        /// Test: Navigate to game with mocked error response 
        /// </summary>
        [Test]
        [Category("Game")]
        public async Task VerifyErrorHandlingOn501Response()
        {
            var errMsg = "505 Internal Server Error.";
            // Intercept and return 501 error response
            await Page.RouteAsync("**/Client/LoadGame", async route =>
            {
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 501,
                    ContentType = "application/json",
                    Body = "{\"error\": \"Not Implemented\"}"
                });
            });

            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94));

            var introPage = new IntroGamePage(Page);
            await introPage.VerifyErrorMessage(errMsg);
        }

        /// <summary>
        /// Test: Perform spin with mocked API response with high win and high balance
        /// </summary>
        [Test]
        [Category("Game")]
        [Description("Verify that user is winning on single run - with mocked API response BIG WIN")]
        public async Task WinSingleSpinBigWinMock()
        {
            var expectedBal = 800001985.00m;
            var expectedWin = 8000.0m;
            // Setup API mock before navigating
            await MocksHelper.SetupCollectWinMock(Page);

            // Navigate to game
            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94));

            // Load and play game
            var introPage = new IntroGamePage(Page);
            await introPage.WaitForProgressBarComplete();
            await introPage.AssertPlayButtonVisibleAndClickable();
            await introPage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var game = new GamePage(Page);
            await game.WaitForGameLoad();

            var balBefore = await game.GetBalance();

            await game.PerformSpin();

            await game.WaitForSpinAnimationComplete();

            var balAfter = await game.GetBalance();

            var win = await game.ControlsTop.WinDisplay.GetAmountDecimal();
            Assert.AreEqual(expectedWin, win);
            Assert.AreEqual(expectedBal, balAfter);
            Assert.AreNotEqual(balBefore, balAfter, "Balance should be updated after win collection");
        }

        [Test]
        [Category("Game")]
        [Description("Run a spin with autoplay and verify correct stake in request")]
        public async Task VerifyAutoSpinFunctionality()
        {
            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94));

            var introPage = new IntroGamePage(Page);
            await introPage.WaitForProgressBarComplete();
            await introPage.AssertPlayButtonVisibleAndClickable();
            await introPage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var game = new GamePage(Page);
            await game.WaitForGameLoad();

            await game.ClickAutoplay();
            await game.ClickPlusStakeAuto(10);

            await game.ClickAutoplayStartButton();

            var balBefore = await game.GetBalance();
            Assert.AreEqual(balBefore, 1975.00m);
            var (req, reqBody) = await PlaywrightHelpers.WaitForRequest(Page, "Client/Action", timeoutMs: 15000);
            Assert.IsTrue(reqBody.Contains("\"Stake\":25"),
                "Request body should contain 'Stake':25");
            await PlaywrightHelpers.WaitForElement(Page, AppConstants.Selectors.MessageDialog);
            var balAfter = await game.GetBalance();
            Assert.AreNotEqual(balAfter, balBefore);
        }

        [Test]
        [Category("Game")]
        [Description("Run a spin with different culture and verify labels")]
        public async Task VerifyGameLocalizationBG()
        {
            var balLabel = "БАЛАНС";
            var winLabel = "ПЕЧАЛБА";
            var culture = "BG";
            var featureLabel = "ФУНКЦИЯ ЗА ЗАКУПУВАНЕ";

            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94, culture));

            var introPage = new IntroGamePage(Page);
            await introPage.WaitForProgressBarComplete();
            await introPage.AssertPlayButtonVisibleAndClickable();
            await introPage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var game = new GamePage(Page);
            await game.WaitForGameLoad();

            await game.ControlsTop.BalanceDisplay.VerifyDisplayLabel(balLabel);
            await game.ControlsTop.WinDisplay.VerifyDisplayLabel(winLabel);
            await game.ControlsBottom.FeatureButtons.VerifyBuyFeatureLabel(featureLabel);

            await game.IncreaseStake(4);
            await game.PerformSpin();
            var bal1 = await game.GetBalance();
            await game.WaitForSpinAnimationComplete();
            var win = await game.GetWin();
            var expectedBal = bal1 + win;
            var finalBal = await game.GetBalance();
            Assert.AreEqual(finalBal, expectedBal, "Balance should be updated by the win amount");
        }

        [Test]
        [Category("Game")]
        [Description("Run a spin with Buy Feature and verify correct win and balance update")]
        public async Task VerifyBuyFeatureFunctionality()
        {
            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94));

            var introPage = new IntroGamePage(Page);
            await introPage.WaitForProgressBarComplete();
            await introPage.AssertPlayButtonVisibleAndClickable();
            await introPage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var game = new GamePage(Page);
            await game.WaitForGameLoad();

            await game.ClickBuyFeature();
            await game.ControlsBottom.FeatureButtons.ClickBuy();
            await game.ControlsBottom.FeatureButtons.WaitForBanner(AppConstants.Texts.CongratulationsText, AppConstants.Texts.WonText, true);
            await game.ControlsBottom.FeatureButtons.ClickBannerStartBtn();
            var balAfterBuy = await game.GetBalance();
            await game.ControlsBottom.FeatureButtons.FreeGamesIndicatorIsVisible();
            await game.ControlsBottom.FeatureButtons.WaitForBanner(AppConstants.Texts.CongratulationsText, AppConstants.Texts.WonText);
            await game.ControlsBottom.FeatureButtons.WaitForBannerToDisappear();

            var win = await game.GetWin();
            var expectedBal = balAfterBuy + win;
            var finalBal = await game.GetBalance();

            Assert.AreEqual(finalBal, expectedBal, "Balance should be updated by the win amount");
            Assert.AreNotEqual(0.0m, win, "Win amount should not be zero");
        }

        [Test]
        [Category("Game")]
        [Description("Verify balance after multiple manual spins")]
        public async Task VerifyBalanceAfterMultipleManualSpins()
        {
            var stake = 5;
            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94));

            var introPage = new IntroGamePage(Page);
            await introPage.WaitForProgressBarComplete();
            await introPage.AssertPlayButtonVisibleAndClickable();
            await introPage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var game = new GamePage(Page);
            await game.WaitForGameLoad();

            await game.IncreaseStake(4);
            await game.PerformSpin();
            var bal1 = await game.GetBalance();
            await game.WaitForSpinAnimationComplete();
            var win1 = await game.GetWin();
            await game.PerformSpin();
            await game.WaitForSpinAnimationComplete();
            var win2 = await game.GetWin();
            var expectedBal = bal1 + win2 + win1 - stake;
            var finalBal = await game.GetBalance();
            Assert.AreEqual(finalBal, expectedBal, "Balance should be updated by the win amount");
        }
    }
}



