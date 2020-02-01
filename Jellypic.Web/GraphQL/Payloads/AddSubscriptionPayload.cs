using GraphQL.Types;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Payloads
{
    public class AddSubscriptionPayload
    {
        public Subscription Subscription { get; set; }
    }

    public class AddSubscriptionPayloadType : ObjectGraphType<AddSubscriptionPayload>
    {
        public AddSubscriptionPayloadType()
        {
            Name = "AddSubscriptionPayload";
            Field(t => t.Subscription, type: typeof(NonNullGraphType<SubscriptionType>));
        }
    }
}
