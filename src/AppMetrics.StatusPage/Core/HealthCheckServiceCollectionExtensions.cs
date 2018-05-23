using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace AppMetrics.StatusPage
{
    public static class HealthCheckServiceCollectionExtensions
    {
        private static readonly Type HealthCheckServiceInterface = typeof(IHealthCheckService);

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, Action<HealthCheckBuilder> checks)
        {
            if (services.Any(descriptor => descriptor.ServiceType == HealthCheckServiceInterface))
            {
                throw new ArgumentException("AddHealthChecks may only be called once.");
            }
            var builder = new HealthCheckBuilder();

            services.AddSingleton<IHealthCheckService, HealthCheckService>(serviceProvider =>
            {
                var serviceScopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
                return new HealthCheckService(builder, serviceProvider, serviceScopeFactory);
            });

            checks(builder);
            return services;
        }
    }
}
