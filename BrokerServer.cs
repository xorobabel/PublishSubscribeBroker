using System;
using System.Collections.Generic;
using System.Text;
using PublishSubscribeBroker.Networking;

namespace PublishSubscribeBroker
{
    // Specialized server class to act as a broker in the publish-subscribe pattern
    class BrokerServer : Server
    {

        // Constructor to build a Broker server with the specified IP and port
        public BrokerServer(string ipAddress, int port) : base(ipAddress, port)
        {
            // TODO
        }

        // TODO

    }
}
