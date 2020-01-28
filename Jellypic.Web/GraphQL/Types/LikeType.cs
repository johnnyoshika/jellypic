using GraphQL.DataLoader;
using GraphQL.Types;
using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class LikeType : ObjectGraphType<Like>
    {
        public LikeType(IBatchLoader batchLoader, IDataLoaderContextAccessor accessor)
        {
            Name = "Like";

            Field(t => t.Id);
            Field(t => t.CreatedAt, type: typeof(DateTimeGraphType));

            Field<NonNullGraphType<UserType>>(
                "user",
                resolve: context =>
                {
                    var loader = accessor.Context.GetOrAddBatchLoader<int, User>("GetUsersByIdsAsync", batchLoader.GetUsersByIdsAsync);
                    return loader.LoadAsync(context.Source.UserId);
                });
        }
    }
}
