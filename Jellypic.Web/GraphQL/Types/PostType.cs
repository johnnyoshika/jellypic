using GraphQL.Types;
using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class PostType : ObjectGraphType<Post>
    {
        public PostType(JellypicContext dataContext)
        {
            Name = "Post";

            Field(t => t.Id);
            Field(t => t.CreatedAt, type: typeof(DateTimeGraphType));
            Field(t => t.CloudinaryPublicId);

            Field<NonNullGraphType<UserType>>(
                "user",
                resolve: context => dataContext
                    .Users
                    .FirstAsync(u => u.Id == context.Source.UserId));

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<LikeType>>>>(
                "likes",
                resolve: context => dataContext
                    .Likes
                    .Where(l => l.PostId == context.Source.Id)
                    .OrderBy(l => l.CreatedAt)
                    .ToListAsync());

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<CommentType>>>>(
                "comments",
                resolve: context => dataContext
                    .Comments
                    .Where(c => c.PostId == context.Source.Id)
                    .OrderBy(c => c.CreatedAt)
                    .ToListAsync());
        }
    }
}
