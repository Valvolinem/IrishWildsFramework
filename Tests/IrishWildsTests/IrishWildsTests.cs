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

            var introGamePage = new IntroGamePage(Page);
            await introGamePage.WaitForProgressBarComplete();
            await introGamePage.AssertPlayButtonVisibleAndClickable();
            await introGamePage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var gamePage = new GamePage(Page);
            await gamePage.WaitForGameLoad();

            await gamePage.ControlsBottom.IsVisible();
            await gamePage.IncreaseStake(6);
            await gamePage.PerformSpin();
            var balanceAfterStake = await gamePage.GetBalance();
            await gamePage.WaitForSpinAnimationComplete();
            var noWin = await gamePage.GetWin();
            Assert.AreEqual(0.0m, noWin);
            var balanceAfterLosingSpin = await gamePage.GetBalance();
            Assert.AreEqual(balanceAfterStake, balanceAfterLosingSpin);
        }

        /// <summary>
        /// Test: Navigate to game with mocked error response 
        /// </summary>
        [Test]
        [Category("Game")]
        public async Task VerifyErrorHandlingOn501Response()
        {
            var errorMessage = "505 Internal Server Error.";
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

            var introGamePage = new IntroGamePage(Page);
            await introGamePage.VerifyErrorMessage(errorMessage);
        }

        /// <summary>
        /// Test: Perform spin with mocked API response with high win and high balance
        /// </summary>
        [Test]
        [Category("Game")]
        [Description("Verify that user is winning on single run - with mocked API response BIG WIN")]
        public async Task WinSingleSpinBigWinMock()
        {
            var finalBalance = 800001985.00m;
            var winAmount = 8000.0m;
            // Setup API mock before navigating
            await MocksHelper.SetupCollectWinMock(Page);

            // Navigate to game
            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94));

            // Load and play game
            var introGamePage = new IntroGamePage(Page);
            await introGamePage.WaitForProgressBarComplete();
            await introGamePage.AssertPlayButtonVisibleAndClickable();
            await introGamePage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var gamePage = new GamePage(Page);
            await gamePage.WaitForGameLoad();

            var balanceBeforeSpin = await gamePage.GetBalance();

            await gamePage.PerformSpin();

            await gamePage.WaitForSpinAnimationComplete();

            var balanceAfterSpin = await gamePage.GetBalance();

            var winAmountAfterSpin = await gamePage.ControlsTop.WinDisplay.GetAmountDecimal();
            Assert.AreEqual(winAmount, winAmountAfterSpin);
            Assert.AreEqual(finalBalance, balanceAfterSpin);
            Assert.AreNotEqual(balanceBeforeSpin, balanceAfterSpin, "Balance should be updated after win collection");
        }

        [Test]
        [Category("Game")]
        [Description("Run a spin with autoplay and verify correct stake in request")]
        public async Task VerifyAutoSpinFunctionality()
        {
            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94));

            var introGamePage = new IntroGamePage(Page);
            await introGamePage.WaitForProgressBarComplete();
            await introGamePage.AssertPlayButtonVisibleAndClickable();
            await introGamePage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var gamePage = new GamePage(Page);
            await gamePage.WaitForGameLoad();

            await gamePage.ClickAutoplay();
            await gamePage.ClickPlusStakeAuto(10);

            await gamePage.ClickAutoplayStartButton();

            var balanceAfterStake = await gamePage.GetBalance();
            Assert.AreEqual(balanceAfterStake, 1975.00m);
            var (actionRequest, actionRequestBody) = await PlaywrightHelpers.WaitForRequest(Page, "Client/Action", timeoutMs: 15000);
            Assert.IsTrue(actionRequestBody.Contains("\"Stake\":25"),
                "Request body should contain 'Stake':25");
            await PlaywrightHelpers.WaitForElement(Page, AppConstants.Selectors.MessageDialog);
            var balanceAfterSpin = await gamePage.GetBalance();
            Assert.AreNotEqual(balanceAfterSpin, balanceAfterStake);
        }

        [Test]
        [Category("Game")]
        [Description("Run a spin with different culture and verify labels")]
        public async Task VerifyGameLocalizationBG()
        {
            var bgBalanceLabel = "БАЛАНС";
            var bgWinLabel = "ПЕЧАЛБА";
            var culture = "BG";
            var featureTextBG = "ФУНКЦИЯ ЗА ЗАКУПУВАНЕ";

            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94, culture));

            var introGamePage = new IntroGamePage(Page);
            await introGamePage.WaitForProgressBarComplete();
            await introGamePage.AssertPlayButtonVisibleAndClickable();
            await introGamePage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var gamePage = new GamePage(Page);
            await gamePage.WaitForGameLoad();

            await gamePage.ControlsTop.BalanceDisplay.VerifyDisplayLabel(bgBalanceLabel);
            await gamePage.ControlsTop.WinDisplay.VerifyDisplayLabel(bgWinLabel);
            await gamePage.ControlsBottom.FeatureButtons.VerifyBuyFeatureLabel(featureTextBG);

            await gamePage.IncreaseStake(4);
            await gamePage.PerformSpin();
            var balanceAfter1Spin = await gamePage.GetBalance();
            await gamePage.WaitForSpinAnimationComplete();
            var winAmount = await gamePage.GetWin();
            var expectedBalance = balanceAfter1Spin + winAmount;
            var finalBalance = await gamePage.GetBalance();
            Assert.AreEqual(finalBalance, expectedBalance, "Balance should be updated by the win amount");
        }

        [Test]
        [Category("Game")]
        [Description("Run a spin with Buy Feature and verify correct win and balance update")]
        public async Task VerifyBuyFeatureFunctionality()
        {
            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94));

            var introGamePage = new IntroGamePage(Page);
            await introGamePage.WaitForProgressBarComplete();
            await introGamePage.AssertPlayButtonVisibleAndClickable();
            await introGamePage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var gamePage = new GamePage(Page);
            await gamePage.WaitForGameLoad();

            await gamePage.ClickBuyFeature();
            await gamePage.ControlsBottom.FeatureButtons.ClickBuy();
            await gamePage.ControlsBottom.FeatureButtons.WaitForBanner(AppConstants.Texts.CongratulationsText, AppConstants.Texts.WonText, true);
            await gamePage.ControlsBottom.FeatureButtons.ClickBannerStartBtn();
            var balanceAfterBuying = await gamePage.GetBalance();
            await gamePage.ControlsBottom.FeatureButtons.FreeGamesIndicatorIsVisible();
            await gamePage.ControlsBottom.FeatureButtons.WaitForBanner(AppConstants.Texts.CongratulationsText, AppConstants.Texts.WonText);
            await gamePage.ControlsBottom.FeatureButtons.WaitForBannerToDisappear();

            var winAmount = await gamePage.GetWin();
            var expectedBalance = balanceAfterBuying + winAmount;
            var finalBalance = await gamePage.GetBalance();

            Assert.AreEqual(finalBalance, expectedBalance, "Balance should be updated by the win amount");
            Assert.AreNotEqual(0.0m, winAmount, "Win amount should not be zero");
        }

        [Test]
        [Category("Game")]
        [Description("Verify balance after multiple manual spins")]
        public async Task VerifyBalanceAfterMultipleManualSpins()
        {
            var stake = 5;
            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IrishWilds94));

            var introGamePage = new IntroGamePage(Page);
            await introGamePage.WaitForProgressBarComplete();
            await introGamePage.AssertPlayButtonVisibleAndClickable();
            await introGamePage.PlayButton.ClickAsync();

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var gamePage = new GamePage(Page);
            await gamePage.WaitForGameLoad();

            await gamePage.IncreaseStake(4);
            await gamePage.PerformSpin();
            var balanceAfter1Spin = await gamePage.GetBalance();
            await gamePage.WaitForSpinAnimationComplete();
            var firstWinAmount = await gamePage.GetWin();
            await gamePage.PerformSpin();
            await gamePage.WaitForSpinAnimationComplete();
            var secondWinAmount = await gamePage.GetWin();
            var ExpectedFinalBalance = balanceAfter1Spin + secondWinAmount + firstWinAmount - stake;
            var FinalBalance = await gamePage.GetBalance();
            Assert.AreEqual(FinalBalance, ExpectedFinalBalance, "Balance should be updated by the win amount");
        }
    }
}



