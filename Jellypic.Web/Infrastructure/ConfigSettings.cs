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

        public ConfigSettings(IConfigurationRoot root)
        {
            Cloudinary = new CloudinarySettings(root.GetSection("Cloudinary"));
        }
        
        public CloudinarySettings Cloudinary { get; }
    }

    public class CloudinarySettings
    {
        public CloudinarySettings(IConfigurationSection section)
        {
            CloudName = section["CloudName"];
        }

        public string CloudName { get; }
    }
}
