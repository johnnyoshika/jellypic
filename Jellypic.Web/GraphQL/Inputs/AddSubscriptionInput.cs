using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Inputs
{
    public class AddSubscriptionInput
    {
        public string Endpoint { get; set; }
        public string P256DH { get; set; }
        public string Auth { get; set; }
    }

    public class AddSubscriptionInputType : InputObjectGraphType
    {
        public AddSubscriptionInputType()
        {
            Name = "AddSubscriptionInput";
            Field<NonNullGraphType<StringGraphType>>("endpoint");
            Field<NonNullGraphType<StringGraphType>>("p256dh");
            Field<NonNullGraphType<StringGraphType>>("auth");
        }
    }
}
