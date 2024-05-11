using Polly;
using Polly.Retry;
using System.Diagnostics.CodeAnalysis;

namespace AudioGeraImagemWorker.Worker.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class PollyConfiguration
    {
        public static void AddRetryPolicy(this IServiceCollection services)
        {
            services.AddSingleton<AsyncPolicy>(
               CreateWaitAndRetryPolicy(new[]
               {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
               }));
        }
        private static AsyncRetryPolicy CreateWaitAndRetryPolicy(IEnumerable<TimeSpan> sleepsBeetweenRetries)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(sleepsBeetweenRetries);
        }
    }
}