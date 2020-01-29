using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Inputs
{
    public class CreatePostInput
    {
        public string CloudinaryPublicId { get; set; }
    }

    public class CreatePostInputType : InputObjectGraphType
    {
        public CreatePostInputType()
        {
            Name = "CreatePostInput";
            Field<NonNullGraphType<StringGraphType>>("cloudinaryPublicId");
        }
    }
}
