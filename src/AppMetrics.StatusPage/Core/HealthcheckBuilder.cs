using System;
using System.Collections.Generic;

namespace AppMetrics.StatusPage
{
    public class HealthCheckBuilder
    {
        public readonly Dictionary<string, HealthCheck> _healthChecks;

        public HealthCheckBuilder()
        {
            _healthChecks = new Dictionary<string, HealthCheck>(StringComparer.OrdinalIgnoreCase);
        }

        public HealthCheckBuilder AddCheck(string checkName, HealthCheck healthCheck)
        {
            if (string.IsNullOrWhiteSpace(checkName))
            {
                throw new ArgumentException("message", nameof(checkName));
            }

            if (healthCheck == null)
            {
                throw new ArgumentNullException(nameof(healthCheck));
            }

            _healthChecks.Add(checkName, healthCheck);
            return this;
        }
    }
}
