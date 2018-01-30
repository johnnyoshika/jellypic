using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Jellypic.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public IActionResult Index() =>
            Content($"Jellypic: {DateTime.Now.ToString("t")}");

        [HttpGet("favicon.ico")]
        public IActionResult Favicon() =>
            File(Convert.FromBase64String(
                "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQEAYAAABPYyMiAAAABmJLR0T///////8JWPfcAAAACXBIWXMAAABIAAAASABGyWs+AAAAF0lEQVRIx2NgGAWjYBSMglEwCkbBSAcACBAAAeaR9cIAAAAASUVORK5CYII="),
                "image/x-icon");
    }
}