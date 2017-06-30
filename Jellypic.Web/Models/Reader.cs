using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Jellypic.Web.Models
{
    public static class Reader
    {
        public static IQueryable<Post> ReadPosts(this JellypicContext context, Expression<Func<Post, bool>> filter) =>
            context
                .Posts
                .Include(p => p.User)
                .Include("Likes.User")
                .Include("Comments.User")
                .Where(filter)
                .OrderByDescending(p => p.Id);
    }
}
