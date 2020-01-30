using GraphQL.Types;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Payloads
{
    public class AddPostPayload
    {
        public Post Subject { get; set; }
    }

    public class AddPostPayloadType : ObjectGraphType<AddPostPayload>
    {
        public AddPostPayloadType()
        {
            Name = "AddPostPayload";
            Field(t => t.Subject, type: typeof(NonNullGraphType<PostType>));
        }
    }
}
