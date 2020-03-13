using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Inputs
{
    public class RemoveCommentInput
    {
        public int Id { get; set; }
    }

    public class RemoveCommentInputType : InputObjectGraphType
    {
        public RemoveCommentInputType()
        {
            Name = "RemoveCommentInput";
            Field<NonNullGraphType<IdGraphType>>("id");
        }
    }
}
