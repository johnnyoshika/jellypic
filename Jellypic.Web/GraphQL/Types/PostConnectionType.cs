using GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class PostConnectionType : ObjectGraphType<PostConnection>
    {
        public PostConnectionType()
        {
            Name = "PostConnection";

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<PostType>>>>(
                "nodes",
                resolve: context => context.Source.Nodes);

            Field<NonNullGraphType<PageInfoType>>(
                "pageInfo",
                resolve: context => context.Source.PageInfo);
        }
    }
}
