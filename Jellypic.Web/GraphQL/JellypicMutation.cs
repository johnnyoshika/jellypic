using GraphQL.Types;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public class JellypicMutation : ObjectGraphType
    {
        public JellypicMutation(IUserLogin userLogin)
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
        }
    }
}
