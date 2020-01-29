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
    public class RemoveCommentPayload
    {
        public int AffectedRows { get; set; }
        public int? PostId { get; set; }
    }

    public class RemoveCommentPayloadType : ObjectGraphType<RemoveCommentPayload>
    {
        public RemoveCommentPayloadType(Func<JellypicContext> dataContext)
        {
            Name = "RemoveCommentPayload";
            Field(t => t.AffectedRows);

            FieldAsync<PostType>(
                "post",
                resolve: async context =>
                {
                    if (!context.Source.PostId.HasValue)
                        return null;

                    using (var dc = dataContext())
                        return await dc.Posts.FirstAsync(p => p.Id == context.Source.PostId);
                });
        }
    }
}
