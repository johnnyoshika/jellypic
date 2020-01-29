using GraphQL.Types;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Payloads
{
    public class CreatePostPayload
    {
        public Post Subject { get; set; }
    }

    public class CreatePostPayloadType : ObjectGraphType<CreatePostPayload>
    {
        public CreatePostPayloadType()
        {
            Name = "CreatePostPayload";
            Field(t => t.Subject, type: typeof(NonNullGraphType<PostType>));
        }
    }
}
