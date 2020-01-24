using GraphQL;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Types;
using Jellypic.Web.GraphQL.Types;
using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public class JellypicQuery : ObjectGraphType
    {
        public JellypicQuery(JellypicContext dataContext, IUserContext userContext)
        {
            this.AuthorizeWith("LoggedIn");

            Field<UserType>(
                "user",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" }),
                resolve: context =>
                {
                    var id = context.GetArgument<object>("id");
                    if (id.ToString() == "me")
                        id = userContext.UserId;

                    if (!int.TryParse(id.ToString(), out int result))
                        context.Errors.Add(new ExecutionError("Invalid 'id'."));

                    return dataContext
                        .Users
                        .FirstOrDefaultAsync(u => u.Id == result);
                });
        }
    }
}
