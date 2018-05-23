using System.Threading.Tasks;
using AppMetrics.StatusPage.Viewmodels;
using Microsoft.AspNetCore.Mvc;

namespace AppMetrics.StatusPage
{
    public class HomeController : Controller
    {
        private readonly IHealthCheckService _healthCheckSvc;
        public HomeController(IHealthCheckService checkSvc)
        {
            _healthCheckSvc = checkSvc;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _healthCheckSvc.CheckHealthAsync();
            var data = new HealthStatusViewModel(result.CheckStatus);

            foreach (var checkResult in result.Results)
            {
                data.AddResult(checkResult.Key, checkResult.Value);
            }

            ViewBag.RefreshSeconds = 60;

            return View(data);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
