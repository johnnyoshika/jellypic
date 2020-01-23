using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Jellypic.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Jellypic.Web.Controllers
{
    [Route("api/sessions")]
    public class SessionController : Controller
    {
        public SessionController(IUserContext userContext, IUserLogin userLogin, JellypicContext dataContext)
        {
            UserContext = userContext;
            UserLogin = userLogin;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; set; }
        IUserLogin UserLogin { get; }
        JellypicContext DataContext { get; set; }

        [Authorize]
        [HttpGet("me")]
        public async Task<object> Get() =>
            (await DataContext
                .Users
                .FirstAsync(u => u.Id == UserContext.UserId))
                .ToJson();

        [HttpPost]
        public async Task Post([FromBody] SessionPostArgs args) =>
            await UserLogin.LogInAsync(args.AccessToken);
    }

    public class SessionPostArgs
    {
        public string AccessToken { get; set; }
    }
}
