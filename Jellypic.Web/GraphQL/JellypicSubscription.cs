using GraphQL.Resolvers;
using GraphQL.Types;
using Jellypic.Web.GraphQL.Payloads;
using Jellypic.Web.GraphQL.Subscriptions;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public class JellypicSubscription : ObjectGraphType
    {
        public JellypicSubscription(PostsAddedSubscriptionService postsAddedSubscription)
        {
            AddField(new EventStreamFieldType
            {
                Name = "postsAdded",
                Type = typeof(NonNullGraphType<ListGraphType<NonNullGraphType<AddPostPayloadType>>>),
                Resolver = new FuncFieldResolver<List<AddPostPayload>>(context => context.Source as List<AddPostPayload>),
                Subscriber = new EventStreamResolver<List<AddPostPayload>>(context => postsAddedSubscription.GetPosts())
            });
        }
    }
}
