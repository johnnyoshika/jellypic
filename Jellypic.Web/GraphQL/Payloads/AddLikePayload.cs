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
        public Like Like { get; set; }
        public Post Post { get; set; }
    }

    public class AddLikePayloadType : ObjectGraphType<AddLikePayload>
    {
        public AddLikePayloadType()
        {
            Name = "AddLikePayload";
            Field(t => t.Like, type: typeof(NonNullGraphType<LikeType>));
            Field(t => t.Post, type: typeof(NonNullGraphType<PostType>));
        }
    }
}
