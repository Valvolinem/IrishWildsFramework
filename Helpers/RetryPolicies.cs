using Polly;

namespace PlaywrightTests.Helpers
{
    public static class RetryPolicies
    {
        /// <summary>
        /// Default retry policy for flaky tests (3)
        /// </summary>
        public static IAsyncPolicy<T> GetDefaultRetryPolicy<T>(string operationName = "Operation")
        {
            return Policy<T>
                .Handle<Exception>()
                .OrResult(r => r == null!)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt =>
                    {
                        var delay = TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100);
                        return delay;
                    }
                );
        }

        /// <summary>
        /// Aggressive retry policy for element clicks (5 retries)
        /// </summary>
        public static IAsyncPolicy<T> GetAggressiveRetryPolicy<T>(string operationName = "Operation")
        {
            return Policy<T>
                .Handle<Exception>()
                .OrResult(r => r == null!)
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                    }
                );
        }

    
        /// <summary>
        /// Execute operation with retry policy (void operation)
        /// </summary>
        public static async Task ExecuteWithRetryAsync(
            Func<Task> operation,
            string operationName = "Operation")
        {
            var policy = GetDefaultRetryPolicy<bool>(operationName);

            await policy.ExecuteAsync(async () =>
            {
                await operation();
                return true;
            });
        }
    }
}
