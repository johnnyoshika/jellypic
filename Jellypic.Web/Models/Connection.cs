using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.Models
{
    public class Connection<T>
    {
        public List<T> Nodes { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
