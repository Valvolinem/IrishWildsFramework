using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Helpers;
using PlaywrightTests.PageObjects.Pages;

namespace PlaywrightTests.Tests.IcyFruits10
{
    [TestFixture]
    public class IcyFruits10Tests : BasePlaywrightTest
    {
        [Test]
        [Category("Game")]
        [Description("Verify balance after multiple manual spins")]
        public async Task VerifyBalanceAfterMultipleManualSpins()
        {
            var stake = 5;
            await Navigate(PlaywrightHelpers.GetGameUrl(GameType.IcyFruits10_95));

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
