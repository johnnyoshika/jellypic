using GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class SubscriptionType : ObjectGraphType<Subscription>
    {
        public SubscriptionType()
        {
            Name = "Subscription";

            Field(t => t.Id, type: typeof(NonNullGraphType<IdGraphType>));
            Field(t => t.CreatedAt, type: typeof(NonNullGraphType<DateTimeGraphType>));
            Field(t => t.Endpoint);
        }
    }
}
