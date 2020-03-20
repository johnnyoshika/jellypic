using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.Services
{
    public interface IEncryptor
    {
        string Encrypt(string text);
        string Decrypt(string text);
        bool TryDecrypt(string text, out string result);
    }
}
