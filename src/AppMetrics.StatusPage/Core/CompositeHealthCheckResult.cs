using System;
using System.Collections.Generic;
using System.Linq;

namespace AppMetrics.StatusPage
{
    public class CompositeHealthCheckResult : IHealthCheckResult
    {
        private static readonly IReadOnlyDictionary<string, object> _emptyData = new Dictionary<string, object>();
        private readonly CheckStatus _initialStatus;
        private readonly CheckStatus _partiallyHealthyStatus;
        private readonly Dictionary<string, IHealthCheckResult> _results = new Dictionary<string, IHealthCheckResult>(StringComparer.OrdinalIgnoreCase);

        public CompositeHealthCheckResult(CheckStatus partiallyHealthyStatus = CheckStatus.Degraded, CheckStatus initialStatus = CheckStatus.Unknown)
        {
            _partiallyHealthyStatus = partiallyHealthyStatus;
            _initialStatus = initialStatus;
        }

        public CheckStatus CheckStatus
        {
            get
            {
                var checkStatuses = new HashSet<CheckStatus>(_results.Select(x => x.Value.CheckStatus));
                if (checkStatuses.Count == 0)
                {
                    return _initialStatus;
                }
                if (checkStatuses.Count == 1)
                {
                    return checkStatuses.First();
                }
                if (checkStatuses.Contains(CheckStatus.Healthy))
                {
                    return _partiallyHealthyStatus;
                }

                return CheckStatus.Unhealthy;
            }
        }

        public string Description => string.Join(Environment.NewLine, _results.Select(r => $"{r.Key}: {r.Value.Description}"));

        public IReadOnlyDictionary<string, object> Data
        {
            get
            {
                var result = new Dictionary<string, object>();

                foreach (var kvp in _results)
                    result.Add(kvp.Key, kvp.Value.Data);

                return result;
            }
        }

        public IReadOnlyDictionary<string, IHealthCheckResult> Results => _results;

        public void Add(string name, CheckStatus status, string description)
            => Add(name, status, description, null);

        public void Add(string name, CheckStatus status, string description, Dictionary<string, object> data)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            _results.Add(name, HealthCheckResult.FromStatus(status, description, data));
        }
        public void Add(string name, IHealthCheckResult checkResult)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (checkResult == null)
            {
                throw new ArgumentNullException(nameof(checkResult));
            }

            _results.Add(name, checkResult);
        }
    }
}
