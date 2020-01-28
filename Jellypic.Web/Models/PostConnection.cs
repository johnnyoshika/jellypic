using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.Models
{
    public class PostConnection
    {
        public List<Post> Nodes { get; set; }
        public PageInfo Page { get; set; }
    }
}
