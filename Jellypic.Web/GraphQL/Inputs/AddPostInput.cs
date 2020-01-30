using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Inputs
{
    public class AddPostInput
    {
        public string CloudinaryPublicId { get; set; }
    }

    public class AddPostInputType : InputObjectGraphType
    {
        public AddPostInputType()
        {
            Name = "AddPostInput";
            Field<NonNullGraphType<StringGraphType>>("cloudinaryPublicId");
        }
    }
}
