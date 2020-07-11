using System;
using PublishSubscribeBroker.Networking;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Data object to hold information about a subscriber
    /// </summary>
    [Serializable]
    public class Subscriber : ClientInfo
    {
        /// <summary>
        /// The readable name of the subscriber
        /// </summary>
        public string Name { get; set; }
    }

    // Specialized client to act as a subscriber in the publish-subscribe pattern
    class SubscriberClient : Client
    {
        public SubscriberClient(string ipAddress, int port) : base(ipAddress, port)
        {

        }

        // TOOD
    }
}
