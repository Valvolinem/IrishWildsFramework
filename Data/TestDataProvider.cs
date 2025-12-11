namespace PlaywrightTests.Data
{
    public class GameTestData
    {
        public string GameName { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
    }

    public static class TestDataProvider
    {
        public static class Games
        {
            public static readonly GameTestData IrishWilds = new()
            {
                GameName = "Irish Wilds",
                Description = "Irish-themed slot game"
            };

            public static readonly GameTestData TunaOverload = new()
            {
                GameName = "Tuna Overload",
                Description = "Ocean-themed slot game"
            };

            public static readonly GameTestData TenxMinimum = new()
            {
                GameName = "10x Minimum",
                Description = "Low minimum bet slot game"
            };

            /// <summary>
            /// Get all available test games
            /// </summary>
            public static IEnumerable<GameTestData> GetAllGames()
            {
                yield return IrishWilds;
                yield return TunaOverload;
                yield return TenxMinimum;
            }

        }
    }
}
