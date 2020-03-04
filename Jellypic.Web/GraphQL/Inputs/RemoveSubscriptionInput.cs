using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Inputs
{
    public class RemoveSubscriptionInput
    {
        public string Endpoint { get; set; }
    }

    public class RemoveSubscriptionInputType : InputObjectGraphType
    {
        public RemoveSubscriptionInputType()
        {
            Name = "RemoveSubscriptionInput";
            Field<NonNullGraphType<StringGraphType>>("endpoint");
        }
    }
}
