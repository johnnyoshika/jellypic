using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Inputs
{
    public class RemoveLikeInput
    {
        public int PostId { get; set; }
    }

    public class RemoveLikeInputType : InputObjectGraphType
    {
        public RemoveLikeInputType()
        {
            Name = "RemoveLikeInput";
            Field<NonNullGraphType<IdGraphType>>("postId");
        }
    }
}
