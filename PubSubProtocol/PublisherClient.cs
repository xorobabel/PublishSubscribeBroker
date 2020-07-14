using System;
using System.Net.Sockets;
using PublishSubscribeBroker.Networking;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Specialized client to act as a publisher in a publish-subscribe system
    /// </summary>
    class PublisherClient : Client
    {
        /// <summary>
        /// The readable name of the publisher client
        /// </summary>
        public string Name { get; set; } = "unnamed";

        public PublisherClient(string ipAddress, int port) : base(ipAddress, port)
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

        // TODO
    }
}
