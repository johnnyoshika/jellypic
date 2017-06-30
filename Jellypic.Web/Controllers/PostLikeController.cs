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
    [Route("api/posts/{id}/likes")]
    public class PostLikeController : Controller
    {
        public PostLikeController(IUserContext userContext, JellypicContext dataContext)
        {
            UserContext = userContext;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; }
        JellypicContext DataContext { get; }

        [HttpPut]
        public async Task<object> Put(int id)
        {
            var like = await ReadLikeAsync(l => l.UserId == UserContext.UserId && l.PostId == id);
            if (like != null)
                return (await ReadPostAsync(id)).ToJson();

            like = new Like
            {
                PostId = id,
                UserId = UserContext.UserId,
                CreatedAt = DateTime.UtcNow
            };

            DataContext.Likes.Add(like);
            await DataContext.SaveChangesAsync();
            return (await ReadPostAsync(id)).ToJson();
        }

        [HttpDelete]
        public async Task<object> Delete(int id)
        {
            var like = await ReadLikeAsync(l => l.PostId == id && l.UserId == UserContext.UserId);
            if (like == null)
                return (await ReadPostAsync(id)).ToJson();

            DataContext.Likes.Remove(like);
            await DataContext.SaveChangesAsync();
            return (await ReadPostAsync(id)).ToJson();
        }

        async Task<Post> ReadPostAsync(int id) =>
            await DataContext
                .ReadPosts(p => p.Id == id)
                .FirstOrDefaultAsync();

        async Task<Like> ReadLikeAsync(Expression<Func<Like, bool>> filter) =>
            await DataContext
                .Likes
                .FirstOrDefaultAsync(filter);
    }
}
