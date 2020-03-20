﻿using GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType(Func<JellypicContext> dataContext)
        {
            Name = "User";

            Field(t => t.Id, type: typeof(IdGraphType));
            Field(t => t.Username);
            Field(t => t.FirstName);
            Field(t => t.LastName);
            Field(t => t.PictureUrl);
            Field(t => t.ThumbUrl);
        }
    }
}