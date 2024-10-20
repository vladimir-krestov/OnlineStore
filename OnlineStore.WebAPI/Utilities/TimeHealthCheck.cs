using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OnlineStore.WebAPI.Utilities
{
    public class TimeHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var currentTime = DateTime.Now.TimeOfDay;

            // Condition: from 7AM till 11PM - Healthy, else - Unhealthy
            var startTime = new TimeSpan(7, 0, 0); // 7:00 AM
            var endTime = new TimeSpan(23, 0, 0);  // 11:00 PM

            return currentTime >= startTime && currentTime <= endTime 
                ? Task.FromResult(HealthCheckResult.Healthy("The time is within acceptable range."))
                : Task.FromResult(HealthCheckResult.Unhealthy("It's night time. The system is considered unhealthy."));
        }
    }
}
