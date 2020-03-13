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
    public class PostType : ObjectGraphType<Post>
    {
        public PostType(IBatchLoader batchLoader, IDataLoaderContextAccessor accessor)
        {
            Name = "Post";

            Field(t => t.Id, type: typeof(IdGraphType));
            Field(t => t.CreatedAt, type: typeof(DateTimeGraphType));
            Field(t => t.CloudinaryPublicId);

            Field<NonNullGraphType<UserType>>(
                "user",
                resolve: context =>
                {
                    var loader = accessor.Context.GetOrAddBatchLoader<int, User>("GetUsersByIdsAsync", batchLoader.GetUsersByIdsAsync);
                    return loader.LoadAsync(context.Source.UserId);
                });

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<LikeType>>>>(
                "likes",
                resolve: context =>
                {
                    var loader = accessor.Context.GetOrAddCollectionBatchLoader<int, Like>("GetLikesByPostIdsAsync", batchLoader.GetLikesByPostIdsAsync);
                    return loader.LoadAsync(context.Source.Id);
                });

            Field<NonNullGraphType<ListGraphType<NonNullGraphType<CommentType>>>>(
                "comments",
                resolve: context =>
                {
                    var loader = accessor.Context.GetOrAddCollectionBatchLoader<int, Comment>("GetCommentsByPostIdsAsync", batchLoader.GetCommentsByPostIdsAsync);
                    return loader.LoadAsync(context.Source.Id);
                });
        }
    }
}
