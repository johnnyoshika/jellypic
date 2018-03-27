using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;

namespace Jellypic.Web.Common
{
    public class CloudinaryUrlBuilder
    {
        public CloudinaryUrlBuilder()
        {
            Cloudinary = new Cloudinary(new Account(
                ConfigSettings.Current.Cloudinary.CloudName,
                ConfigSettings.Current.Cloudinary.ApiKey,
                ConfigSettings.Current.Cloudinary.ApiSecret));
        }

        Cloudinary Cloudinary { get; set; }

        public string Square(string publicId, int size) =>
            Cloudinary.Api.UrlImgUp
                .Secure(true)
                .Transform(new Transformation().Width(size).Height(size).Crop("fill"))
                .BuildUrl(publicId);

        public string FourThreeRatio(string publicId, int width) =>
            Cloudinary.Api.UrlImgUp
                .Secure(true)
                .Transform(new Transformation().Width(width).Height(width * 3 / 4).Crop("fill"))
                .BuildUrl(publicId);

        public string FacebookUser(string facebookUserId, int size) =>
            Cloudinary.Api.UrlImgUp
                .Secure(true)
                .Action("facebook")
                .Transform(new Transformation()
                    .Width(size)
                    .Height(size)
                    .Crop("fill")
                    .Gravity("auto:faces")
                    .Radius("max"))
                .BuildUrl($"{facebookUserId}.png");
    }
}
