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
        public Post Post { get; set; }
    }

    public class RemoveCommentPayloadType : ObjectGraphType<RemoveCommentPayload>
    {
        public RemoveCommentPayloadType()
        {
            Name = "RemoveCommentPayload";
            Field(t => t.AffectedRows);
            Field(t => t.Post, type: typeof(NonNullGraphType<PostType>));
        }
    }
}
