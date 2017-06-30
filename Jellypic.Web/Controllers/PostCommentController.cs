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
    [Route("api/posts/{id}/comments")]
    public class PostCommentController : Controller
    {
        public PostCommentController(IUserContext userContext, JellypicContext dataContext)
        {
            UserContext = userContext;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; }
        JellypicContext DataContext { get; }

        [HttpPost]
        public async Task<object> Post(int id, [FromBody]CommentPostArgs data)
        {
            var comment = new Comment
            {
                PostId = id,
                UserId = UserContext.UserId,
                Text = data.Text,
                CreatedAt = DateTime.UtcNow
            };

            DataContext.Comments.Add(comment);
            await DataContext.SaveChangesAsync();
            return (await ReadPostAsync(id)).ToJson();
        }

        [HttpDelete("{commentId}")]
        public async Task<object> Delete(int id, int commentId)
        {
            var comment = await ReadAsync(c => c.Id == commentId && c.UserId == UserContext.UserId);
            if (comment == null)
                return (await ReadPostAsync(id)).ToJson();

            DataContext.Comments.Remove(comment);
            await DataContext.SaveChangesAsync();
            return (await ReadPostAsync(id)).ToJson();
        }

        async Task<Post> ReadPostAsync(int id) =>
            await DataContext
                .ReadPosts(p => p.Id == id)
                .FirstOrDefaultAsync();

        async Task<Comment> ReadAsync(Expression<Func<Comment, bool>> filter) =>
            await DataContext
                .Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(filter);
    }

    public class CommentPostArgs
    {
        public string Text { get; set; }
    }
}
