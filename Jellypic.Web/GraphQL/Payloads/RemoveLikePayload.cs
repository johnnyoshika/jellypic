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
    public class RemoveLikePayload
    {
        public int AffectedRows { get; set; }
        public int PostId { get; set; }
    }

    public class RemoveLikePayloadType : ObjectGraphType<RemoveLikePayload>
    {
        public RemoveLikePayloadType(Func<JellypicContext> dataContext)
        {
            Name = "RemoveLikePayload";
            Field(t => t.AffectedRows);

            FieldAsync<NonNullGraphType<PostType>>(
                "post",
                resolve: async context =>
                {
                    using (var dc = dataContext())
                        return await dc.Posts.FirstAsync(p => p.Id == context.Source.PostId);
                });
        }
    }
}
