using GraphQL;
using GraphQL.Types;
using Jellypic.Web.GraphQL.Types;
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
        public JellypicQuery(JellypicContext dataContext)
        {
            Field<UserType>(
                "user",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>>
                    {
                        Name = "id"
                    }),
                resolve: context =>
                {
                    int id = context.GetArgument<int>("id");
                    if (id < 1)
                        context.Errors.Add(new ExecutionError("'id' out of range."));

                    return dataContext
                        .Users
                        .FirstOrDefaultAsync(u => u.Id == id);
                });
        }
    }
}
