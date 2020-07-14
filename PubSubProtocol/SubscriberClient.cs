using System;
using System.Net.Sockets;
using PublishSubscribeBroker.Networking;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Specialized client to act as a subscriber in a publish-subscribe system
    /// </summary>
    class SubscriberClient : Client
    {
        /// <summary>
        /// The readable name of the subscriber client
        /// </summary>
        public string Name { get; set; } = "unnamed";

        public SubscriberClient(string ipAddress, int port) : base(ipAddress, port)
        {
            // TODO
        }

        /// <summary>
        /// Override method to handle the publish-subscribe protocol on the subscriber side
        /// </summary>
        /// <param name="stream">The network stream used for communication with the server</param>
        protected override void HandleProtocol(NetworkStream stream)
        {
            // TODO
        }

        // TOOD
    }
}
