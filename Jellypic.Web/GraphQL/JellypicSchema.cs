using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public class JellypicSchema : Schema
    {
        public JellypicSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<JellypicQuery>();
            Mutation = resolver.Resolve<JellypicMutation>();
        }
    }
}
