using GraphQL.Types;
using Jellypic.Web.GraphQL.Inputs;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Jellypic.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public class JellypicMutation : ObjectGraphType
    {
        public JellypicMutation(IUserLogin userLogin, IUserContext userContext, Func<JellypicContext> dataContext)
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
        }
    }
}
