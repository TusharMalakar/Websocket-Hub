using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalrAPI.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : Controller
    {
        public string Index()
        {
            return "SIGNAL-R API IS UP...";
        }
    }
}
