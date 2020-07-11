using System;
using PublishSubscribeBroker.Networking;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Data object to hold information about a publisher
    /// </summary>
    [Serializable]
    public class Publisher : ClientInfo
    {
        /// <summary>
        /// The readable name of the publisher
        /// </summary>
        public string Name { get; set; }
    }

    // Specialized client to act as a publisher in the publish-subscribe pattern
    class PublisherClient : Client
    {
        public PublisherClient(string ipAddress, int port) : base(ipAddress, port)
        {

        }

        // TODO
    }
}
