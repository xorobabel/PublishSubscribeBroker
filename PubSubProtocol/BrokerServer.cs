using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using PublishSubscribeBroker.Networking;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Specialized server class to act as a message broker in a publish-subscribe system
    /// </summary>
    class BrokerServer : Server
    {
        /// <summary>
        /// A thread-safe collection of all active topics, organized by the unique ID of each topic
        /// </summary>
        protected ConcurrentDictionary<Guid, Topic> topics;

        /// <summary>
        /// Constructor to build a publish-subscribe broker server with the specified IP and port
        /// </summary>
        public BrokerServer(string ipAddress, int port) : base(ipAddress, port)
        {
            topics = new ConcurrentDictionary<Guid, Topic>();
        }

        /// <summary>
        /// Override method to handle the publish-subscribe protocol on the server/broker side
        /// </summary>
        /// <param name="stream">The network stream used for communication with the client</param>
        protected override void HandleProtocol(NetworkStream stream)
        {
            // TODO
        }

    }
}
