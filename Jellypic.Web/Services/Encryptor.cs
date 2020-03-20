using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Jellypic.Web.Services
{
    public class Encryptor : IEncryptor
    {
        public string Encrypt(string text)
        {
            using (var algorithm = CreateAlgorithm())
            {
                var encryptor = algorithm.CreateEncryptor();

                using (var memory = new MemoryStream())
                using (var crypto = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                {
                    using (var writer = new StreamWriter(crypto))
                        writer.Write(text);

                    return ByteArrayToString(memory.ToArray());
                }
            }
        }

        public string Decrypt(string text) =>
            TryDecrypt(text, out string result)
                ? result
                : throw new CryptographicException();

        public bool TryDecrypt(string text, out string result)
        {
            result = null;
            if (text == null)
                return false;

            try
            {
                using (var algorithm = CreateAlgorithm())
                {
                    var decryptor = algorithm.CreateDecryptor();

                    using (var memory = new MemoryStream(StringToByteArray(text)))
                    using (var crypto = new CryptoStream(memory, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(crypto))
                        result = reader.ReadToEnd();

                    return true;
                }
            }
            catch (FormatException) { } // FormatException if text is not a hex value
            catch (ArgumentOutOfRangeException) { } // ArgumentOutOfRangeException if text is less than 16 characters
            catch (CryptographicException) { } // CryptographicException if text is invalid

            return false;
        }

        Aes CreateAlgorithm()
        {
            var algorithm = Aes.Create();
            algorithm.Mode = CipherMode.ECB;
            algorithm.Padding = PaddingMode.PKCS7;
            algorithm.BlockSize = 128;
            algorithm.KeySize = 128;

            // Key generation: https://www.codeproject.com/Questions/1254466/How-to-do-AES-key-generation-in-Csharp
            algorithm.Key = Convert.FromBase64String(ConfigSettings.Current.Encryptor.Key);
            return algorithm;
        }

        string ByteArrayToString(byte[] bytes) =>
            bytes.Select(b => b.ToString("X2")).Aggregate((s1, s2) => s1 + s2);

        byte[] StringToByteArray(string hex) =>
            Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
    }
}
