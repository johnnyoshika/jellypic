using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.Services
{
    public interface ISignIn
    {
        Task SignInAsync(string userId);
    }
}
