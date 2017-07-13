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
            var post = await ReadPostAsync(id);
            var like = await ReadLikeAsync(l => l.UserId == UserContext.UserId && l.PostId == id);
            if (like != null)
                return post.ToJson();

            DataContext.Likes.Add(new Like
            {
                PostId = id,
                UserId = UserContext.UserId,
                CreatedAt = DateTime.UtcNow
            });

            if (UserContext.UserId != post.UserId)
                DataContext.Notifications.Add(new Notification
                {
                    ActorId = UserContext.UserId,
                    PostId = id,
                    RecipientId = post.UserId,
                    CreatedAt = DateTime.UtcNow,
                    Type = NotificationType.Like
                });

            await DataContext.SaveChangesAsync();
            return post.ToJson();
        }

        [HttpDelete]
        public async Task<object> Delete(int id)
        {
            var post = await ReadPostAsync(id);
            var like = await ReadLikeAsync(l => l.PostId == id && l.UserId == UserContext.UserId);
            if (like == null)
                return post.ToJson();

            DataContext.Likes.Remove(like);
            DataContext.Notifications.RemoveRange(await DataContext.Notifications.Where(n =>
                n.Type == NotificationType.Like && n.ActorId == UserContext.UserId && n.PostId == id).ToArrayAsync());
            await DataContext.SaveChangesAsync();
            return post.ToJson();
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
