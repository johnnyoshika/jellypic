using GraphQL.Types;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Payloads
{
    public class LoginPayload
    {
        public string AuthToken { get; set; }
        public User Me { get; set; }
    }

    public class LoginPayloadType : ObjectGraphType<LoginPayload>
    {
        public LoginPayloadType()
        {
            Name = "LoginPayload";
            Field(t => t.AuthToken);
            Field(t => t.Me, type: typeof(NonNullGraphType<UserType>));
        }
    }
}
