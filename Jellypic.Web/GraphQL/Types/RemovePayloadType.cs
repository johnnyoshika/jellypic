using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class RemovePayload
    {
        public bool Success { get; set; }
    }

    public class RemovePayloadType : ObjectGraphType<RemovePayload>
    {
        public RemovePayloadType()
        {
            Name = "RemovePayload";
            Field(t => t.Success);
        }
    }
}
