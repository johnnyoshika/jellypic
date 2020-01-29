using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Inputs
{
    public class LoginInput
    {
        public string AccessToken { get; set; }
    }

    public class LoginInputType : InputObjectGraphType
    {
        public LoginInputType()
        {
            Name = "LoginInput";
            Field<NonNullGraphType<StringGraphType>>("accessToken");
        }
    }
}
