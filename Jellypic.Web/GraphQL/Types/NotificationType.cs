using GraphQL.DataLoader;
using GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class NotificationType : ObjectGraphType<Notification>
    {
        public NotificationType(IBatchLoader batchLoader, IDataLoaderContextAccessor accessor)
        {
            Name = "Notification";

            Field(t => t.Id, type: typeof(IdGraphType));
            Field(t => t.CreatedAt, type: typeof(DateTimeGraphType));

            Field<NonNullGraphType<UserType>>(
                "actor",
                resolve: context =>
                {
                    var loader = accessor.Context.GetOrAddBatchLoader<int, User>("GetUsersByIdsAsync", batchLoader.GetUsersByIdsAsync);
                    return loader.LoadAsync(context.Source.ActorId);
                });

            Field<NonNullGraphType<PostType>>(
                "post",
                resolve: context =>
                {
                    var loader = accessor.Context.GetOrAddBatchLoader<int, Post>("GetPostsByIdsAsync", batchLoader.GetPostsByIdsAsync);
                    return loader.LoadAsync(context.Source.PostId);
                });
        }
    }
}
