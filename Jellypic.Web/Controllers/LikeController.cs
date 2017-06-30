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
    [Route("api/likes")]
    public class LikeController : Controller
    {
        public LikeController(IUserContext userContext, JellypicContext dataContext)
        {
            UserContext = userContext;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; }
        JellypicContext DataContext { get; }

        [HttpPost]
        public async Task<object> Post([FromBody]LikesPostArgs data)
        {
            var like = await Read(l => l.UserId == UserContext.UserId && l.PostId == data.PostId);
            if (like != null)
                return like.ToJson();

            like = new Like
            {
                PostId = data.PostId,
                UserId = UserContext.UserId,
                CreatedAt = DateTime.UtcNow
            };

            DataContext.Likes.Add(like);
            await DataContext.SaveChangesAsync();
            return (await Read(l => l.Id == like.Id)).ToJson();
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var like = await Read(l => l.Id == id && l.UserId == UserContext.UserId);

            if (like == null)
                throw new NotFoundException($"Like {id} for user {UserContext.UserId} not found.");

            DataContext.Likes.Remove(like);
            await DataContext.SaveChangesAsync();
        }

        async Task<Like> Read(Expression<Func<Like, bool>> filter) =>
            await DataContext
                .Likes
                .Include(l => l.User)
                .FirstOrDefaultAsync(filter);
    }

    public class LikesPostArgs
    {
        public int PostId { get; set; }
    }
}
