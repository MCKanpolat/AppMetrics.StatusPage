using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppMetrics.StatusPage
{
    public static partial class HealthCheckBuilderExtensions
    {
        public static HealthCheckBuilder AddUrlCheck(this HealthCheckBuilder builder, string url)
      => AddUrlCheck(builder, url, response => UrlChecker.DefaultUrlCheck(response));

        public static HealthCheckBuilder AddUrlCheck(this HealthCheckBuilder builder, string url, Func<HttpResponseMessage, IHealthCheckResult> checkFunc)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddUrlCheck(builder, url, checkFunc);
        }

        public static HealthCheckBuilder AddUrlCheck(this HealthCheckBuilder builder, string url, Func<HttpResponseMessage, Task<IHealthCheckResult>> checkFunc)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddUrlCheck(builder, url, checkFunc);
        }

        public static HealthCheckBuilder AddUrlCheck(this HealthCheckBuilder builder, string url, Func<HttpResponseMessage, ValueTask<IHealthCheckResult>> checkFunc)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (checkFunc == null)
            {
                throw new ArgumentNullException(nameof(checkFunc));
            }

            builder.AddCheck($"UrlCheck({url})", HealthCheck.FromTaskCheck(() =>
            {
                var urlCheck = new UrlChecker(checkFunc, url);
                return urlCheck.CheckAsync();
            }));
            return builder;
        }



        public static HealthCheckBuilder AddAppMetricsCheck(this HealthCheckBuilder builder, string name, string url)
 => AddAppMetricsCheck(builder, name, url, response => AppmetricsChecker.DefaultAppmetricsCheck(response));

        public static HealthCheckBuilder AddAppMetricsCheck(this HealthCheckBuilder builder, string url, Func<HttpResponseMessage, IHealthCheckResult> checkFunc)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddAppMetricsCheck(builder, url, checkFunc);
        }

        public static HealthCheckBuilder AddAppMetricsCheck(this HealthCheckBuilder builder, string url, Func<HttpResponseMessage, Task<IHealthCheckResult>> checkFunc)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddAppMetricsCheck(builder, url, checkFunc);
        }

        public static HealthCheckBuilder AddAppMetricsCheck(this HealthCheckBuilder builder, string name, string url, Func<HttpResponseMessage, ValueTask<IHealthCheckResult>> checkFunc)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (checkFunc == null)
            {
                throw new ArgumentNullException(nameof(checkFunc));
            }

            builder.AddCheck($"Check({name})", HealthCheck.FromTaskCheck(() =>
            {
                var appmetricsChecker = new AppmetricsChecker(checkFunc, url);
                return appmetricsChecker.CheckAsync();
            }));
            return builder;
        }

    }
}
