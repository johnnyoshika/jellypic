using GraphQL.Types;
using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class LikeType : ObjectGraphType<Like>
    {
        public LikeType(JellypicContext dataContext)
        {
            Name = "Like";

            Field(t => t.Id);
            Field(t => t.CreatedAt, type: typeof(DateTimeGraphType));

            Field<NonNullGraphType<UserType>>(
                "user",
                resolve: context => dataContext
                        .Users
                        .FirstAsync(u => u.Id == context.Source.UserId));
        }
    }
}
