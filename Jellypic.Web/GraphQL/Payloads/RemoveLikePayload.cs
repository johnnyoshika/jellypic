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
        public Post Post { get; set; }
    }

    public class RemoveLikePayloadType : ObjectGraphType<RemoveLikePayload>
    {
        public RemoveLikePayloadType()
        {
            Name = "RemoveLikePayload";
            Field(t => t.AffectedRows);
            Field(t => t.Post, type: typeof(NonNullGraphType<PostType>));
        }
    }
}
