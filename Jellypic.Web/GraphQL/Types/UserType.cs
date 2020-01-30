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

            Field(t => t.Id, type: typeof(IdGraphType));
            Field(t => t.Username);
            Field(t => t.FirstName);
            Field(t => t.LastName);
            Field(t => t.PictureUrl);
            Field(t => t.ThumbUrl);

            FieldAsync<NonNullGraphType<PostConnectionType>>(
                "posts",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "after" }),
                resolve: async context =>
                {
                    int? after = context.GetArgument<int?>("after");
                    return await dataContext.PostConnectionAsync(p =>
                        p.UserId == context.Source.Id
                        &&
                        (!after.HasValue || p.Id < after));
                });
        }
    }
}
