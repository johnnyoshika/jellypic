using GraphQL.Types;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Payloads
{
    public class AddLikePayload
    {
        public Like Subject { get; set; }
    }

    public class AddLikePayloadType : ObjectGraphType<AddLikePayload>
    {
        public AddLikePayloadType(Func<JellypicContext> dataContext)
        {
            Name = "AddLikePayload";
            Field(t => t.Subject, type: typeof(NonNullGraphType<LikeType>));

            FieldAsync<NonNullGraphType<PostType>>(
                "post",
                resolve: async context =>
                {
                    using (var dc = dataContext())
                        return await dc.Posts.FirstAsync(p => p.Id == context.Source.Subject.PostId);
                });
        }
    }
}
