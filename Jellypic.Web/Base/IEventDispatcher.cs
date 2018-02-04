using System.Threading.Tasks;
using Jellypic.Web.Events;

namespace Jellypic.Web.Base
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(DomainEvent e);
    }
}