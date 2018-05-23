using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppMetrics.StatusPage
{
    public class HealthCheck : IHealthCheck
    {
        protected HealthCheck(Func<CancellationToken, ValueTask<IHealthCheckResult>> check)
        {
            Check = check ?? throw new ArgumentNullException(nameof(check));
        }

        protected Func<CancellationToken, ValueTask<IHealthCheckResult>> Check { get; }

        public ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
            => Check(cancellationToken);

        public static HealthCheck FromCheck(Func<IHealthCheckResult> check)
            => new HealthCheck(token => new ValueTask<IHealthCheckResult>(check()));

        public static HealthCheck FromCheck(Func<CancellationToken, IHealthCheckResult> check)
            => new HealthCheck(token => new ValueTask<IHealthCheckResult>(check(token)));

        public static HealthCheck FromTaskCheck(Func<Task<IHealthCheckResult>> check)
            => new HealthCheck(token => new ValueTask<IHealthCheckResult>(check()));

        public static HealthCheck FromTaskCheck(Func<CancellationToken, Task<IHealthCheckResult>> check)
            => new HealthCheck(token => new ValueTask<IHealthCheckResult>(check(token)));

        public static HealthCheck FromValueTaskCheck(Func<ValueTask<IHealthCheckResult>> check)
            => new HealthCheck(token => check());

        public static HealthCheck FromValueTaskCheck(Func<CancellationToken, ValueTask<IHealthCheckResult>> check)
            => new HealthCheck(check);
    }
}
