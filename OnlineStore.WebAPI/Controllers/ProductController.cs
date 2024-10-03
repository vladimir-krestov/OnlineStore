using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetAllProducts")]
        public IEnumerable<WeatherForecast> GetAll()
        {
        }
    }
}
