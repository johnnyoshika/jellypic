using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public int PostCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}
