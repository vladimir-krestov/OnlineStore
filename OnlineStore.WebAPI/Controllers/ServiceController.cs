using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.WebAPI.Utilities;

namespace OnlineStore.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly FactorialBackgroundService _backgroundService;

        public ServiceController(FactorialBackgroundService backgroundService)
        {
            _backgroundService = backgroundService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route(nameof(GetHealthcheckResult))]
        public Task<ActionResult> GetHealthcheckResult()
        {
            return Task.FromResult(Ok() as ActionResult);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(StopBackgroundService))]
        public ActionResult StopBackgroundService()
        {
            _backgroundService.StopService();

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(RestartBackgroundService))]
        public ActionResult RestartBackgroundService()
        {
            _backgroundService.RestartService();

            return Ok();
        }
    }
}
