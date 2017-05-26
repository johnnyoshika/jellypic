using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Jellypic.Web.Controllers
{
    [Route("api/[controller]")]
    public class SessionsController : Controller
    {
        [HttpGet("me")]
        public IActionResult Get()
        {
            return Content("Hello world!");
        }
    }
}
