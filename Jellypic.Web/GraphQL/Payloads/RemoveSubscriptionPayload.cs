using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Payloads
{
    public class RemoveSubscriptionPayload
    {
        public int AffectedRows { get; set; }
    }

    public class RemoveSubscriptionPayloadType : ObjectGraphType<RemoveSubscriptionPayload>
    {
        public RemoveSubscriptionPayloadType()
        {
            Name = "RemoveSubscriptionPayload";
            Field(t => t.AffectedRows);
        }
    }
}
