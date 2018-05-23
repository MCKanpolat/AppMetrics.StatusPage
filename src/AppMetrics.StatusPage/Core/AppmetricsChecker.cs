using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using App.Metrics.Health.Formatters.Json;
using App.Metrics.Health.Formatters.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AppMetrics.StatusPage
{
    public class AppmetricsChecker
    {
        private readonly Func<HttpResponseMessage, ValueTask<IHealthCheckResult>> _checkFunc;
        private readonly string _url;

        public AppmetricsChecker(Func<HttpResponseMessage, ValueTask<IHealthCheckResult>> checkFunc, string url)
        {
            _checkFunc = checkFunc ?? throw new ArgumentNullException(nameof(checkFunc));
            _url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public CheckStatus PartiallyHealthyStatus { get; set; } = CheckStatus.Degraded;

        public async Task<IHealthCheckResult> CheckAsync()
        {
            using (var httpClient = CreateHttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(_url).ConfigureAwait(false);
                    return await _checkFunc(response);
                }
                catch (Exception ex)
                {
                    var data = new Dictionary<string, object> { { "url", _url } };
                    return HealthCheckResult.Unhealthy($"Exception during check: {ex.GetType().FullName}", data);
                }
            }
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = GetHttpClient();
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            return httpClient;
        }

        private static readonly DefaultContractResolver _sharedContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        public static async ValueTask<IHealthCheckResult> DefaultAppmetricsCheck(HttpResponseMessage response)
        {
            var status = CheckStatus.Unknown;
            var jsonText = await response.Content.ReadAsStringAsync();

            var data = new Dictionary<string, object>
            {
                { "url", response.RequestMessage.RequestUri.ToString() },
                { "status", (int)response.StatusCode },
                { "reason", response.ReasonPhrase }
            };

            var healthStatusData = JsonConvert.DeserializeObject<HealthStatusData>(jsonText, new HealthStatusConverter());
            if (healthStatusData != null)
            {
                status = Enum.Parse<CheckStatus>(healthStatusData.Status);
                foreach (var item in healthStatusData.Unhealthy)
                {
                    data.Add($"Unhealthy - {item.Key}", item.Value);
                }
                foreach (var item in healthStatusData.Degraded)
                {
                    data.Add($"Degraded - {item.Key}", item.Value);
                }
                foreach (var item in healthStatusData.Healthy)
                {
                    data.Add($"Healthy - {item.Key}", item.Value);
                }
                foreach (var item in healthStatusData.Ignored)
                {
                    data.Add($"Ignored - {item.Key}", item.Value);
                }
            }
            return HealthCheckResult.FromStatus(status, $"status code {response.StatusCode} ({(int)response.StatusCode})", data);
        }

        protected virtual HttpClient GetHttpClient() => new HttpClient();
    }
}
