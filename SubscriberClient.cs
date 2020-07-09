using System;
using System.Collections.Generic;
using System.Text;
using PublishSubscribeBroker.Networking;

namespace PublishSubscribeBroker
{
    // Specialized client to act as a subscriber in the publish-subscribe pattern
    class SubscriberClient : Client
    {
        public SubscriberClient(string ipAddress, int port) : base(ipAddress, port)
        {

        }

        // TOOD
    }
}
