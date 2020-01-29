using GraphQL;
using GraphQL.Types;
using Jellypic.Web.Common;
using Jellypic.Web.GraphQL.Inputs;
using Jellypic.Web.GraphQL.Payloads;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Jellypic.Web.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public class JellypicMutation : ObjectGraphType
    {
        public JellypicMutation(IUserLogin userLogin, IUserContext userContext, Func<JellypicContext> dataContext, INotificationCreator notificationCreator)
        {
            FieldAsync<UserType>(
                "login",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "accessToken" }),
                resolve: async context =>
                {
                    string accessToken = context.GetArgument<string>("accessToken");
                    return await userLogin.LogInAsync(accessToken);
                });

            FieldAsync<NonNullGraphType<ListGraphType<NonNullGraphType<PostType>>>>(
                "createPost",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<ListGraphType<NonNullGraphType<CreatePostInputType>>>> { Name = "inputs" }),
                resolve: async context =>
                {
                    var inputs = context.GetArgument<List<CreatePostInput>>("inputs");
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

                        return posts;
                    }
                });

            FieldAsync<AddLikePayloadType>(
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
                            return new AddLikePayload { Subject = like };

                        like = new Like
                        {
                            PostId = post.Id,
                            UserId = userContext.UserId,
                            CreatedAt = DateTime.UtcNow
                        };
                        dc.Likes.Add(like);

                        await dc.SaveChangesAsync();

                        if (userContext.UserId != post.UserId)
                            await notificationCreator.CreateAsync(userContext.UserId, post, NotificationType.Like);

                        return new AddLikePayload { Subject = like };
                    }
                });

            FieldAsync<RemoveLikePayloadType>(
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
                            return new RemoveLikePayload { AffectedRows = 0, PostId = post.Id };

                        dc.Likes.Remove(like);

                        await dc.SaveChangesAsync();

                        return new RemoveLikePayload { AffectedRows = 1, PostId = post.Id };
                    }
                });

            FieldAsync<AddCommentPayloadType>(
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

                        if (userContext.UserId != post.UserId)
                            await notificationCreator.CreateAsync(userContext.UserId, post, NotificationType.Comment);

                        return new AddCommentPayload { Subject = comment };
                    }
                });

            FieldAsync<RemoveCommentPayloadType>(
                "removeComment",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<RemoveCommentInputType>> { Name = "input" }),
                resolve: async context =>
                {
                    var input = context.GetArgument<RemoveCommentInput>("input");

                    using (var dc = dataContext())
                    {
                        var comment = await dc.Comments.FirstOrDefaultAsync(c => c.UserId == userContext.UserId && c.Id == input.Id);
                        if (comment == null)
                            return new RemoveCommentPayload { AffectedRows = 0 };

                        dc.Comments.Remove(comment);
                        await dc.SaveChangesAsync();
                        return new RemoveCommentPayload { AffectedRows = 1, PostId = comment.PostId };
                    }
                });
        }
    }
}
