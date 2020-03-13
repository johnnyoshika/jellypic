using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.Models
{
    public class PageInfo
    {
        public int? EndCursor { get; set; }
        public bool HasNextPage { get; set; }
    }
}
