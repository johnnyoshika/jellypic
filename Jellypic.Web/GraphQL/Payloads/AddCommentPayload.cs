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
        public Post Post { get; set; }
    }

    public class AddCommentPayloadType : ObjectGraphType<AddCommentPayload>
    {
        public AddCommentPayloadType()
        {
            Name = "AddCommentPayload";
            Field(t => t.Comment, type: typeof(NonNullGraphType<CommentType>));
            Field(t => t.Post, type: typeof(NonNullGraphType<PostType>));
        }
    }
}
