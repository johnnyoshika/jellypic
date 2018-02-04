using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Jellypic.Web.Events;

namespace Jellypic.Web.Base
{
    public interface IEventHandler<in T> 
        where T : DomainEvent
    {
        Task HandleAsync(T e);
    }
}
