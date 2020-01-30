using GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class PostConnectionType : ConnectionType<PostConnection, Post, PostType>
    {
        public override string TypeName => "PostConnection";
    }
}
