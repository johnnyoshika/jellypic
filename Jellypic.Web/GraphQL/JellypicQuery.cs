﻿using GraphQL;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Types;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public class JellypicQuery : ObjectGraphType
    {
        public JellypicQuery(Func<JellypicContext> dataContext, IUserContext userContext)
        {
            FieldAsync<UserType>(
                "user",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
                resolve: async context =>
                {
                    var id = context.GetArgument<object>("id");
                    if (id.ToString() == "me")
                        id = userContext.UserId;

                    if (!int.TryParse(id.ToString(), out int result))
                        context.Errors.Add(new ExecutionError("Invalid 'id'."));

                    using (var dc = dataContext())
                        return await dc
                            .Users
                            .FirstOrDefaultAsync(u => u.Id == result);
                }).AuthorizeWith("LoggedIn");

            FieldAsync<ProfileType>(
                "profile",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
                resolve: async context =>
                {
                    var id = context.GetArgument<int>("id");

                    using (var dc = dataContext())
                    return new Profile
                    {
                        Id = id,
                        PostCount = await dc.Posts.CountAsync(p => p.UserId == id),
                        LikeCount = await dc.Likes.CountAsync(l => l.UserId == id),
                        CommentCount = await dc.Comments.CountAsync(c => c.UserId == id)
                    };
                }).AuthorizeWith("LoggedIn");

            FieldAsync<PostType>(
                "post",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
                resolve: async context =>
                {
                    var id = context.GetArgument<int>("id");
                    
                    using (var dc = dataContext())
                        return await dc.Posts.FirstOrDefaultAsync(p => p.Id == id);
                }).AuthorizeWith("LoggedIn");

            FieldAsync<NonNullGraphType<PostConnectionType>>(
                "posts",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "userId" },
                    new QueryArgument<IntGraphType> { Name = "after" }),
                resolve: async context =>
                {
                    int? userId = context.GetArgument<int?>("userId");
                    int? after = context.GetArgument<int?>("after");
                    return await dataContext.PostConnectionAsync(p => 
                        (!userId.HasValue || p.UserId == userId) &&
                        (!after.HasValue || p.Id < after));
                }).AuthorizeWith("LoggedIn");

            FieldAsync<SubscriptionType>(
                "subscription",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "endpoint" }),
                resolve: async context =>
                {
                    using (var dc = dataContext())
                    {
                        string endpoint = context.GetArgument<string>("endpoint");
                        var subscription = await dc.Subscriptions.FirstOrDefaultAsync(s => s.Endpoint == endpoint);

                        if (subscription == null)
                            return null;

                        if (subscription.UserId != userContext.UserId)
                            throw new UnauthorizedAccessException();

                        return subscription;
                    }
                }).AuthorizeWith("LoggedIn");

            FieldAsync<NonNullGraphType<NotificationConnectionType>>(
                "notifications",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "after" }),
                resolve: async context =>
                {
                    int? after = context.GetArgument<int?>("after");
                    return await dataContext.NotificationConnectionAsync(n => !after.HasValue || n.Id < after);
                }).AuthorizeWith("LoggedIn");
        }
    }
}
