using System;
using System.Collections.Generic;
using System.Text;
using PublishSubscribeBroker.Networking;

namespace PublishSubscribeBroker
{
    // Specialized client to act as a publisher in the publish-subscribe pattern
    class PublisherClient : Client
    {
        public PublisherClient(string ipAddress, int port) : base(ipAddress, port)
        {

        }

        // TODO
    }
}
