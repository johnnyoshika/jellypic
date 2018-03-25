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
    [Route("api/profiles")]
    public class ProfileController : Controller
    {
        public ProfileController(JellypicContext dataContext)
        {
            DataContext = dataContext;
        }

        JellypicContext DataContext { get; }

        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            var posts = DataContext.Posts.CountAsync(p => p.UserId == id);
            var likes = DataContext.Likes.CountAsync(l => l.UserId == id);
            var comments = DataContext.Comments.CountAsync(c => c.UserId == id);

            return new
            {
                id,
                postCount = await posts,
                likeCount = await likes,
                commentCount = await comments
            };
        }
    }
}
