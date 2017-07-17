using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jellypic.Web.Controllers
{
    [Authorize]
    [Route("api/notifications")]
    public class NotificationController : Controller
    {
        public NotificationController(IUserContext userContext, JellypicContext dataContext)
        {
            UserContext = userContext;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; }
        JellypicContext DataContext { get; }

        [HttpGet]
        public async Task<object> Get() =>
            await DataContext
                .Notifications
                .Include(n => n.Post)
                .Include(n => n.Actor)
                .Where(n => n.RecipientId == UserContext.UserId && !n.Dismissed)
                .Select(n => n.ToJson())
                .ToListAsync();
    }
}
