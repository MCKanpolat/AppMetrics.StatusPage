using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AppMetrics.StatusPage
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly HealthCheckBuilder _builder;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public HealthCheckService(HealthCheckBuilder builder, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory)
        {
            _builder = builder;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<CompositeHealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var scope = GetServiceScope())
            {
                var result = new CompositeHealthCheckResult();
                var scopeServiceProvider = scope.ServiceProvider;
                var checkTasks = GetAllChecks().Select(check => new { Name = check.Item1, Check = check, Task = RunCheckAsync(check.Item2, cancellationToken).AsTask() }).ToList();

                await Task.WhenAll(checkTasks.Select(checkTask => checkTask.Task));

                foreach (var check in checkTasks)
                {
                    result.Add(check.Name, check.Task.Result);
                }

                return result;
            }
        }

        public async ValueTask<IHealthCheckResult> RunCheckAsync(HealthCheck healthCheck, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await healthCheck.CheckAsync(cancellationToken).ConfigureAwait(false);
        }

        public IReadOnlyList<(string, HealthCheck)> GetAllChecks() => _builder._healthChecks.Select(w => (w.Key, w.Value)).ToList().AsReadOnly();

        public HealthCheck GetCheck(string checkName)
        {
            return _builder._healthChecks[checkName];
        }

        private IServiceScope GetServiceScope()
            => _serviceScopeFactory == null ? new UnscopedServiceProvider(_serviceProvider) : _serviceScopeFactory.CreateScope();

        private class UnscopedServiceProvider : IServiceScope
        {
            public UnscopedServiceProvider(IServiceProvider serviceProvider)
                => ServiceProvider = serviceProvider;

            public IServiceProvider ServiceProvider { get; }

            public void Dispose() { }
        }
    }
}
