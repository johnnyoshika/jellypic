﻿using GraphQL.DataLoader;
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
        public PostType(Func<JellypicContext> dataContext, IBatchLoader batchLoader, IDataLoaderContextAccessor accessor)
        {
            Name = "Post";

            Field(t => t.Id);
            Field(t => t.CreatedAt, type: typeof(DateTimeGraphType));
            Field(t => t.CloudinaryPublicId);

            Field<NonNullGraphType<UserType>>(
                "user",
                resolve: context =>
                {
                    var loader = accessor.Context.GetOrAddBatchLoader<int, User>("GetUsersByIdsAsync", batchLoader.GetUsersByIdsAsync);
                    return loader.LoadAsync(context.Source.UserId);
                });

            FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<LikeType>>>>(
                "likes",
                resolve: async context =>
                {
                    using (var dc = dataContext())
                        return await dc
                            .Likes
                            .Where(l => l.PostId == context.Source.Id)
                            .OrderBy(l => l.CreatedAt)
                            .ToListAsync();
                });

            FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<CommentType>>>>(
                "comments",
                resolve: async context =>
                {
                    using (var dc = dataContext())
                        return await dc
                            .Comments
                            .Where(c => c.PostId == context.Source.Id)
                            .OrderBy(c => c.CreatedAt)
                            .ToListAsync();
                });
        }
    }
}
