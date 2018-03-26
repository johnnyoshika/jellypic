using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellypic.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jellypic.Web.Controllers
{
    [Authorize]
    [Route("api/users")]
    public class UserController : Controller
    {
        public UserController(JellypicContext dataContext)
        {
            DataContext = dataContext;
        }

        JellypicContext DataContext { get; set; }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<object> Get(int id) =>
            (await DataContext
                .Users
                .Include(u => u.Posts)
                .FirstAsync(u => u.Id == id))
                .ToJson();
    }
}
