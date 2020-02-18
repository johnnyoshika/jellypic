using GraphQL.Types;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Payloads
{
    public class UpdatePostPayload
    {
        public Post Post { get; set; }
    }

    public class UpdatePostPayloadType : ObjectGraphType<UpdatePostPayload>
    {
        public UpdatePostPayloadType()
        {
            Name = "UpdatePostPayload";
            Field(t => t.Post, type: typeof(NonNullGraphType<PostType>));
        }
    }
}
