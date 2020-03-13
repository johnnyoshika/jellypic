using GraphQL.Resolvers;
using GraphQL.Types;
using Jellypic.Web.GraphQL.Payloads;
using Jellypic.Web.GraphQL.Subscriptions;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public class JellypicSubscription : ObjectGraphType
    {
        public JellypicSubscription(PostsAddedSubscriptionService postsAddedSubscription, PostUpdatedSubscriptionService postUpdatedSubscription)
        {
            AddField(new EventStreamFieldType
            {
                Name = "postsAdded",
                Arguments = new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "userId" }),
                Type = typeof(NonNullGraphType<ListGraphType<NonNullGraphType<AddPostPayloadType>>>),
                Resolver = new FuncFieldResolver<List<AddPostPayload>>(context => context.Source as List<AddPostPayload>),
                Subscriber = new EventStreamResolver<List<AddPostPayload>>(context => {
                    var userId = context.GetArgument<int?>("userId");
                    return postsAddedSubscription.GetPosts().Where(posts => !userId.HasValue || posts.Any(p => p.Post.UserId == userId));
                })
            });

            AddField(new EventStreamFieldType
            {
                Name = "postUpdated",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
                Type = typeof(NonNullGraphType<UpdatePostPayloadType>),
                Resolver = new FuncFieldResolver<UpdatePostPayload>(context => context.Source as UpdatePostPayload),
                Subscriber = new EventStreamResolver<UpdatePostPayload>(context =>
                {
                    var id = context.GetArgument<int>("id");
                    return postUpdatedSubscription.GetPost().Where(p => p.Post.Id == id);
                })
            });
        }
    }
}
