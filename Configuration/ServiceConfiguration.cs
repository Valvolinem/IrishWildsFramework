using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlaywrightTests.Configuration;
using PlaywrightTests.Services;

namespace PlaywrightTests.Configuration
{
    /// <summary>
    /// Configures dependency injection for the test framework
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Registers all application services into the DI container
        /// </summary>
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register browser service as singleton (pool management)
            services.AddSingleton<IBrowserService>(sp =>
                new BrowserService());

            // Load from appsettings.json and bind to TestConfig
            services.AddSingleton(LoadTestConfig());

            var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = false,
                ValidateOnBuild = false
            });

            return serviceProvider;
        }

        /// <summary>
        /// Loads test configuration from appsettings.json and environment variables
        /// </summary>
        private static TestConfig LoadTestConfig()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            var config = builder.Build();
            var testConfig = new TestConfig();
            config.GetSection("Playwright").Bind(testConfig);
            return testConfig;
        }
    }
}
