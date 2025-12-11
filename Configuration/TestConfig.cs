namespace PlaywrightTests.Configuration
{
    /// <summary>
    /// Configuration values are loaded from appsettings.json and registered in ServiceConfiguration.cs
    /// </summary>
    public class TestConfig
    {
        // Browser settings
        public string? Browser { get; set; }
        public bool Headless { get; set; }
        public int SlowMo { get; set; }
        public string? Device { get; set; } 
        public int NavigationTimeout { get; set; }
        public int ActionTimeout { get; set; }
    }
}
