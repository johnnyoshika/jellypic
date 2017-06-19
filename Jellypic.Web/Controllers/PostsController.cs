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
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        public PostsController(IUserContext userContext, JellypicContext dataContext)
        {
            UserContext = userContext;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; set; }
        JellypicContext DataContext { get; set; }

        [HttpGet]
        public async Task<object> Index(int? after = null)
        {
            int take = 20;

            bool hasMore = DataContext
                .Posts
                .Where(p => !after.HasValue || p.Id > after)
                .OrderByDescending(p => p.Id)
                .Skip(take)
                .Any();

            var posts = await DataContext
                .Posts
                .Include(p => p.User)
                .Include("Likes.User")
                .Include("Comments.User")
                .Where(p => !after.HasValue || p.Id > after)
                .OrderByDescending(p => p.Id)
                .Take(take)
                .ToListAsync();

            return new
            {
                Data = posts.Select(p => p.ToJson()),
                Pagination = new
                {
                    NextUrl = hasMore ? $"/api/posts?after={posts.Last().Id}" : null
                }

            };
        }
    }
}
