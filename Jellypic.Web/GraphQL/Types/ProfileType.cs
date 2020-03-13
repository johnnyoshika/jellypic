using GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class ProfileType : ObjectGraphType<Profile>
    {
        public ProfileType()
        {
            Name = "Profile";

            Field(t => t.Id, type: typeof(IdGraphType));
            Field(t => t.PostCount);
            Field(t => t.LikeCount);
            Field(t => t.CommentCount);
        }
    }
}
