using GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class PageInfoType : ObjectGraphType<PageInfo>
    {
        public PageInfoType()
        {
            Name = "PageInfo";

            Field(t => t.EndCursor, type: typeof(IntGraphType));
            Field(t => t.HasNextPage);
        }
    }
}
