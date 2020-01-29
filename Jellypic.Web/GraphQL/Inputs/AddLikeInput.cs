using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Inputs
{
    public class AddLikeInput
    {
        public int PostId { get; set; }
    }

    public class AddLikeInputType : InputObjectGraphType
    {
        public AddLikeInputType()
        {
            Name = "AddLikeInput";
            Field<NonNullGraphType<IdGraphType>>("postId");
        }
    }
}
