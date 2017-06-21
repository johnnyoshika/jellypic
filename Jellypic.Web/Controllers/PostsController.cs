using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public async Task<object> Get(int? after = null)
        {
            int take = 20;

            bool hasMore = await Read(p => !after.HasValue || p.Id > after)
                .Skip(take)
                .AnyAsync();

            var posts = await Read(p => !after.HasValue || p.Id > after)
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

        [HttpPost]
        public async Task<object> Post([FromBody]IEnumerable<PostsPostArgs> data)
        {
            var posts = new List<Post>();
            foreach (var post in data)
                posts.Add(new Post
                {
                    CreatedAt = DateTime.UtcNow,
                    CloudinaryPublicId = post.CloudinaryPublicId,
                    UserId = UserContext.UserId
                });

            DataContext.Posts.AddRange(posts);
            await DataContext.SaveChangesAsync();

            return new
            {
                Data = (await Read(p => posts.Select(x => x.Id).Contains(p.Id))
                        .ToListAsync())
                    .Select(p => p.ToJson())

            };
        }

        IQueryable<Post> Read(Expression<Func<Post, bool>> filter) =>
            DataContext
                .Posts
                .Include(p => p.User)
                .Include("Likes.User")
                .Include("Comments.User")
                .Where(filter)
                .OrderByDescending(p => p.Id);
    }

    public class PostsPostArgs
    {
        public string CloudinaryPublicId { get; set; }
    }
}
