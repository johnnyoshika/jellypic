using Jellypic.Web.GraphQL.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Subscriptions
{
    public class PostUpdatedSubscriptionService
    {
        readonly ISubject<UpdatePostPayload> _postStream = new ReplaySubject<UpdatePostPayload>(1);

        public void Notify(UpdatePostPayload payload) => _postStream.OnNext(payload); // Will send message to all subscribers

        public IObservable<UpdatePostPayload> GetPost() => _postStream.AsObservable();
    }
}
