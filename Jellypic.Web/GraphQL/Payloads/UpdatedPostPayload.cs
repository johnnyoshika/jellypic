using GraphQL.Types;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Payloads
{
    public class UpdatedPostPayload
    {
        public Post Post { get; set; }
    }

    public class UpdatedPostPayloadType : ObjectGraphType<UpdatedPostPayload>
    {
        public UpdatedPostPayloadType()
        {
            Name = "UpdatedPostPayload";
            Field(t => t.Post, type: typeof(NonNullGraphType<PostType>));
        }
    }
}
