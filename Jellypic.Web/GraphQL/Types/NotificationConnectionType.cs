using GraphQL.Types;
using Jellypic.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Types
{
    public class NotificationConnectionType : ConnectionType<NotificationConnection, Notification, NotificationType>
    {
        public override string TypeName => "NotificationConnection";
    }
}
