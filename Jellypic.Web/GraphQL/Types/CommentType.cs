using GraphQL.Types;
using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class CommentType : ObjectGraphType<Comment>
    {
        public CommentType(JellypicContext dataContext)
        {
            Name = "Comment";

            Field(t => t.Id);
            Field(t => t.CreatedAt, type: typeof(DateTimeGraphType));
            Field(t => t.Text);
            
            Field<NonNullGraphType<UserType>>(
                "user",
                resolve: context => dataContext
                        .Users
                        .FirstAsync(u => u.Id == context.Source.UserId));
        }
    }
}
