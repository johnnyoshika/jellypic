using GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType(Func<JellypicContext> dataContext)
        {
            Name = "User";

            Field(t => t.Id, type: typeof(NonNullGraphType<IdGraphType>));
            Field(t => t.Username);
            Field(t => t.FirstName, type: typeof(StringGraphType));
            Field(t => t.LastName, type: typeof(StringGraphType));
            Field(t => t.PictureUrl, type: typeof(StringGraphType));
            Field(t => t.ThumbUrl, type: typeof(StringGraphType));
        }
    }
}
