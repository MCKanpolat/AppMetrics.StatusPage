using System.Threading;
using System.Threading.Tasks;

namespace AppMetrics.StatusPage
{
    public interface IHealthCheck
    {
        ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
