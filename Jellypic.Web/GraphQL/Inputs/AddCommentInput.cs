using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Inputs
{
    public class AddCommentInput
    {
        public int PostId { get; set; }
        public string Text { get; set; }
    }

    public class AddCommentInputType : InputObjectGraphType
    {
        public AddCommentInputType()
        {
            Name = "AddCommentInput";
            Field<NonNullGraphType<IdGraphType>>("postId");
            Field<NonNullGraphType<StringGraphType>>("text");
        }
    }
}
