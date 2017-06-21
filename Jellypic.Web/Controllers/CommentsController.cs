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
    public class CommentsController : Controller
    {
        public CommentsController(IUserContext userContext, JellypicContext dataContext)
        {
            UserContext = userContext;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; }
        JellypicContext DataContext { get; }

        [HttpPost]
        public async Task<object> Post([FromBody]CommentsPostArgs data)
        {
            var comment = new Comment
            {
                PostId = data.PostId,
                UserId = UserContext.UserId,
                Text = data.Text,
                CreatedAt = DateTime.UtcNow
            };

            DataContext.Comments.Add(comment);
            await DataContext.SaveChangesAsync();
            return (await Read(c => c.Id == comment.Id)).ToJson();
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var comment = await Read(c => c.Id == id && c.UserId == UserContext.UserId);

            if (comment == null)
                throw new NotFoundException($"Comment {id} for user {UserContext.UserId} not found.");

            DataContext.Comments.Remove(comment);
            await DataContext.SaveChangesAsync();
        }

        async Task<Comment> Read(Expression<Func<Comment, bool>> filter) =>
            await DataContext
                .Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(filter);
    }

    public class CommentsPostArgs
    {
        public int PostId { get; set; }
        public string Text { get; set; }
    }
}
