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
        public Post Subject { get; set; }
    }

    public class UpdatedPostPayloadType : ObjectGraphType<UpdatedPostPayload>
    {
        public UpdatedPostPayloadType()
        {
            Name = "UpdatedPostPayload";
            Field(t => t.Subject, type: typeof(NonNullGraphType<PostType>));
        }
    }
}
