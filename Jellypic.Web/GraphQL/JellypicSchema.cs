using GraphQL;
using GraphQL.Types;
using GraphQL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public class JellypicSchema : Schema
    {
        public JellypicSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<JellypicQuery>();
            Mutation = provider.GetRequiredService<JellypicMutation>();
            Subscription = provider.GetRequiredService<JellypicSubscription>();
        }
    }
}
