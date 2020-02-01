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
    public class AddCommentPayload
    {
        public Comment Comment { get; set; }
    }

    public class AddCommentPayloadType : ObjectGraphType<AddCommentPayload>
    {
        public AddCommentPayloadType(Func<JellypicContext> dataContext)
        {
            Name = "AddCommentPayload";
            Field(t => t.Comment, type: typeof(NonNullGraphType<CommentType>));

            FieldAsync<NonNullGraphType<PostType>>(
                "post",
                resolve: async context =>
                {
                    using (var dc = dataContext())
                        return await dc.Posts.FirstAsync(p => p.Id == context.Source.Comment.PostId);
                });
        }
    }
}
