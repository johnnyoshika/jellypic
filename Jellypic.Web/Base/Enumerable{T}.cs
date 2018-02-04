using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Jellypic.Web.Base
{
    public abstract class Enumerable<T> : IEnumerable<T>
    {
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
