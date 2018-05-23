using System.Threading;
using System.Threading.Tasks;

namespace AppMetrics.StatusPage
{
    public interface IHealthCheckService
    {
        Task<CompositeHealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
