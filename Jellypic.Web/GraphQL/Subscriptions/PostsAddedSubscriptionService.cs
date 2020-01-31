using Jellypic.Web.GraphQL.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL.Subscriptions
{
    public class PostsAddedSubscriptionService
    {
        readonly ISubject<List<AddPostPayload>> _postStream = new ReplaySubject<List<AddPostPayload>>(1);

        public void Notify(List<AddPostPayload> payloads) => _postStream.OnNext(payloads); // Will send message to all subscribers

        public IObservable<List<AddPostPayload>> GetPosts() => _postStream.AsObservable();
    }
}
