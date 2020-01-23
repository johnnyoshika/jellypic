using Jellypic.Web.Models;
using System.Threading.Tasks;

namespace Jellypic.Web.Services
{
    public interface IUserLogin
    {
        Task<User> LogInAsync(string facebookAccessToken);
    }
}