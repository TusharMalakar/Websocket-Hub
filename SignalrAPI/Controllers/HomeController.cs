using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SignalrAPI.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return "Zola SignalR Is Running....";
        }
    }
}
