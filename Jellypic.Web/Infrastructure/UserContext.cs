using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Jellypic.Web.Infrastructure
{
    public interface IUserContext
    {
        int UserId { get; }
    }

    public class UserContext : IUserContext
    {
        public const string UserItem = "userid-sDj8#dR!D";

        public UserContext(IHttpContextAccessor accessor)
        {
            Accessor = accessor;
        }

        IHttpContextAccessor Accessor { get; }

        public int UserId => (int)Accessor.HttpContext.Items[UserItem];
    }
}
