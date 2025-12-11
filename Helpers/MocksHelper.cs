using Microsoft.Playwright;

namespace PlaywrightTests.Helpers
{
    public class MocksHelper
    {
        public static string LoadMockResponse(string filePath)
        {
            return System.IO.File.ReadAllText(filePath);
        }

        public static async Task SetupCollectWinMock(IPage page)
        {
            const string LoadGameUrl = "https://rgstorgs.stage.pariplaygames.com/Client/LoadGame";
            var loadGameResponse = LoadMockResponse("Data/Mocks/IrishWilds/Requests/LoadGameResponse.json");
            var winningSpinResponse = LoadMockResponse("Data/Mocks/IrishWilds/Requests/SpinWin080.json");
            var collectResponse = LoadMockResponse("Data/Mocks/IrishWilds/Requests/Collect080.json");
            const string EmptySuccessResponse = "";

            const string ActionUrl = "https://rgstorgs.stage.pariplaygames.com/Client/Action*";
            const string BalanceUrl = "https://rgstorgs.stage.pariplaygames.com/Client/GetBalance*";
            const string EndGameUrl = "https://rgstorgs.stage.pariplaygames.com/Client/EndGame*";

            await page.RouteAsync(LoadGameUrl, async route =>
            {
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = loadGameResponse
                });
            });

            int actionCallCount = 0;
            await page.RouteAsync(ActionUrl, async route =>
            {
                actionCallCount++;
                string jsonResponse = actionCallCount switch
                {
                    1 => winningSpinResponse,
                    2 => collectResponse,
                    _ => collectResponse
                };

                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = jsonResponse
                });
            });

            await page.RouteAsync(BalanceUrl, async route =>
            {
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    Body = EmptySuccessResponse
                });
            });

            await page.RouteAsync(EndGameUrl, async route =>
            {
                await route.FulfillAsync(new RouteFulfillOptions
                {
                    Status = 200,
                    Body = EmptySuccessResponse
                });
            });
        }
    }
}
