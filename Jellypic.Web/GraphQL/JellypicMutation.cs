﻿using GraphQL;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Types;
using Jellypic.Web.Common;
using Jellypic.Web.GraphQL.Inputs;
using Jellypic.Web.GraphQL.Payloads;
using Jellypic.Web.GraphQL.Subscriptions;
using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Jellypic.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public class JellypicMutation : ObjectGraphType
    {
        public JellypicMutation(
            IUserLogin userLogin,
            IUserContext userContext,
            Func<JellypicContext> dataContext,
            INotificationCreator notificationCreator,
            PostsAddedSubscriptionService postsAddedSubscription,
            PostUpdatedSubscriptionService postUpdatedSubscription,
            IEncryptor encryptor,
            IHttpContextAccessor accessor)
        {
            FieldAsync<NonNullGraphType<LoginPayloadType>>(
                "login",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<LoginInputType>> { Name = "input" }),
                resolve: async context =>
                {
                    var input = context.GetArgument<LoginInput>("input");
                    var me = await userLogin.LogInAsync(input.Token);
                    return new LoginPayload
                    {
                        AuthToken = encryptor.Encrypt(me.Id.ToString()),
                        Me = me
                    };
                });

            FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<AddPostPayloadType>>>>(
                "addPosts",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ListGraphType<NonNullGraphType<AddPostInputType>>>> { Name = "inputs" }),
                resolve: async context =>
                {
                    var inputs = context.GetArgument<List<AddPostInput>>("inputs");
                    var posts = inputs.Select(i => new Post
                    {
                        CreatedAt = DateTime.UtcNow,
                        CloudinaryPublicId = i.CloudinaryPublicId,
                        UserId = userContext.UserId
                    }).ToList(); // project to list, otherwise Id won't be set on SaveChangesAsync()

                    using (var dc = dataContext())
                    {
                        dc.Posts.AddRange(posts);
                        await dc.SaveChangesAsync();
                        var payloads = posts.Select(p => new AddPostPayload { Post = p }).ToList();
                        postsAddedSubscription.Notify(payloads);
                        return payloads;
                    }
                }).AuthorizeWith("LoggedIn");

            FieldAsync<NonNullGraphType<AddLikePayloadType>>(
                "addLike",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AddLikeInputType>> { Name = "input" }),
                resolve: async context =>
                {
                    var input = context.GetArgument<AddLikeInput>("input");

                    using (var dc = dataContext())
                    {
                        var post = await dc.Posts.Where(p => p.Id == input.PostId).FirstOrDefaultAsync();
                        if (post == null)
                            throw new ExecutionError($"postId '{input.PostId}' not found.");

                        var like = await dc.Likes.FirstOrDefaultAsync(l => l.UserId == userContext.UserId && l.PostId == post.Id);
                        if (like != null)
                            return new AddLikePayload { Like = like, Post = post };

                        like = new Like
                        {
                            PostId = post.Id,
                            UserId = userContext.UserId,
                            CreatedAt = DateTime.UtcNow
                        };
                        dc.Likes.Add(like);

                        await dc.SaveChangesAsync();

                        postUpdatedSubscription.Notify(new UpdatePostPayload { Post = post });

                        if (userContext.UserId != post.UserId)
                            await notificationCreator.CreateAsync(userContext.UserId, post, Models.NotificationType.Like);

                        return new AddLikePayload { Like = like, Post = post };
                    }
                }).AuthorizeWith("LoggedIn");

            FieldAsync<NonNullGraphType<RemoveLikePayloadType>>(
                "removeLike",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<RemoveLikeInputType>> { Name = "input" }),
                resolve: async context =>
                {
                    var input = context.GetArgument<RemoveLikeInput>("input");

                    using (var dc = dataContext())
                    {
                        var post = await dc.Posts.Where(p => p.Id == input.PostId).FirstOrDefaultAsync();
                        if (post == null)
                            throw new ExecutionError($"postId '{input.PostId}' not found.");

                        var like = await dc.Likes.FirstOrDefaultAsync(l => l.UserId == userContext.UserId && l.PostId == post.Id);
                        if (like == null)
                            return new RemoveLikePayload { AffectedRows = 0, Post = post };

                        dc.Likes.Remove(like);

                        await dc.SaveChangesAsync();
                        
                        postUpdatedSubscription.Notify(new UpdatePostPayload { Post = post });

                        return new RemoveLikePayload { AffectedRows = 1, Post = post };
                    }
                }).AuthorizeWith("LoggedIn");

            FieldAsync<NonNullGraphType<AddCommentPayloadType>>(
                "addComment",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AddCommentInputType>> { Name = "input" }),
                resolve: async context =>
                {
                    var input = context.GetArgument<AddCommentInput>("input");

                    using (var dc = dataContext())
                    {
                        var post = await dc.Posts.Where(p => p.Id == input.PostId).FirstOrDefaultAsync();
                        if (post == null)
                            throw new ExecutionError($"postId '{input.PostId}' not found.");

                        var comment = new Comment
                        {
                            PostId = post.Id,
                            UserId = userContext.UserId,
                            Text = input.Text,
                            CreatedAt = DateTime.UtcNow
                        };

                        dc.Comments.Add(comment);

                        await dc.SaveChangesAsync();

                        postUpdatedSubscription.Notify(new UpdatePostPayload { Post = post });

                        if (userContext.UserId != post.UserId)
                            await notificationCreator.CreateAsync(userContext.UserId, post, Models.NotificationType.Comment);

                        return new AddCommentPayload { Comment = comment, Post = post };
                    }
                }).AuthorizeWith("LoggedIn");

            FieldAsync<NonNullGraphType<RemoveCommentPayloadType>>(
                "removeComment",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<RemoveCommentInputType>> { Name = "input" }),
                resolve: async context =>
                {
                    var input = context.GetArgument<RemoveCommentInput>("input");

                    using (var dc = dataContext())
                    {
                        var comment = await dc.Comments
                            .Include(c => c.Post)
                            .FirstOrDefaultAsync(c => c.UserId == userContext.UserId && c.Id == input.Id);

                        if (comment == null)
                            throw new ExecutionError($"Comment id '{input.Id}' not found.");

                        dc.Comments.Remove(comment);
                        await dc.SaveChangesAsync();

                        postUpdatedSubscription.Notify(new UpdatePostPayload { Post = comment.Post });

                        return new RemoveCommentPayload { AffectedRows = 1, Post = comment.Post };
                    }
                }).AuthorizeWith("LoggedIn");

            FieldAsync<NonNullGraphType<AddSubscriptionPayloadType>>(
                "addSubscription",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AddSubscriptionInputType>> { Name = "input" }),
                resolve: async context =>
                {
                    var input = context.GetArgument<AddSubscriptionInput>("input");

                    using (var dc = dataContext())
                    {
                        var subscription = new Subscription
                        {
                            UserId = userContext.UserId,
                            Endpoint = input.Endpoint,
                            P256DH = input.P256DH,
                            Auth = input.Auth,
                            CreatedAt = DateTime.UtcNow,
                            UserAgent = accessor.HttpContext.Request.Headers["User-Agent"].ToString()
                        };

                        dc.Subscriptions.Add(subscription);
                        await dc.SaveChangesAsync();
                        return new AddSubscriptionPayload { Subscription = subscription };
                    }
                }).AuthorizeWith("LoggedIn");

            FieldAsync<NonNullGraphType<RemoveSubscriptionPayloadType>>(
                "removeSubscription",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<RemoveSubscriptionInputType>> { Name = "input" }),
                resolve: async context =>
                {
                    var input = context.GetArgument<RemoveSubscriptionInput>("input");

                    using (var dc = dataContext())
                    {
                        var subscriptions = await dc.Subscriptions.Where(s => s.Endpoint == input.Endpoint && s.UserId == userContext.UserId).ToListAsync();
                        if (!subscriptions.Any())
                            return new RemoveSubscriptionPayload { AffectedRows = 0 };

                        dc.Subscriptions.RemoveRange(subscriptions);
                        await dc.SaveChangesAsync();
                        return new RemoveSubscriptionPayload { AffectedRows = 1 };
                    }
                }).AuthorizeWith("LoggedIn");
        }
    }
}
