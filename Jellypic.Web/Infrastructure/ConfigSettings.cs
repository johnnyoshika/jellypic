using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Jellypic.Web
{
    public class ConfigSettings
    {
        public static ConfigSettings Current;

        public ConfigSettings(IConfiguration configuration)
        {
            Cloudinary = new Cloudinary(configuration.GetSection("Cloudinary"));
            WebPushVapidKeys = new WebPushVapidKeys(configuration.GetSection("WebPushVapidKeys"));
            Auth0 = new Auth0(configuration.GetSection("Auth0"));
            Encryptor = new Encryptor(configuration.GetSection("Encryptor"));
        }

        public Cloudinary Cloudinary { get; }
        public WebPushVapidKeys WebPushVapidKeys { get; }
        public Auth0 Auth0 { get; }
        public Encryptor Encryptor { get; }
    }

    public class Cloudinary
    {
        public Cloudinary(IConfigurationSection section)
        {
            CloudName = section["CloudName"];
            ApiKey = section["ApiKey"];
            ApiSecret = section["ApiSecret"];
        }

        public string CloudName { get; }
        public string ApiKey { get; }
        public string ApiSecret { get; }
    }

    public class WebPushVapidKeys
    {
        public WebPushVapidKeys(IConfigurationSection section)
        {
            PublicKey = section["PublicKey"];
            PrivateKey = section["PrivateKey"];
        }

        public string PublicKey { get; }
        public string PrivateKey { get; }
    }

    public class Auth0
    {
        public Auth0(IConfigurationSection section)
        {
            Domain = section["Domain"];
            ClientId = section["ClientId"];
        }

        public string Domain { get; }
        public string ClientId { get; }
    }

    public class Encryptor
    {
        public Encryptor(IConfigurationSection section)
        {
            Key = section["Key"];
        }

        public string Key { get; }
    }
}
