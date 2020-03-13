using GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public abstract class ConnectionType<TConnection, TModel, TGraph> 
        : ObjectGraphType<TConnection> 
        where TConnection : Connection<TModel>
        where TGraph : GraphType
    {
        public ConnectionType()
        {
            Name = TypeName;

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<TGraph>>>>(
                "nodes",
                resolve: context => context.Source.Nodes);

            Field<NonNullGraphType<PageInfoType>>(
                "pageInfo",
                resolve: context => context.Source.PageInfo);
        }

        public abstract string TypeName { get; }
    }
}
