using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.Services
{
    public interface IAuth0TokenReader
    {
        Task<JwtSecurityToken> ReadAsync(string idToken);
        Task<string> ReaderUserIdAsync(string idToken);
    }
}
