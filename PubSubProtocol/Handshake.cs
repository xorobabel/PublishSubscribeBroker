using PublishSubscribeBroker.Networking;
using System;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Data object used for the initial handshake when a publisher or subscriber connects to the broker server
    /// </summary>
    public class Handshake
    {
        /// <summary>
        /// Whether the connecting client is a publisher (if not, it must be a subscriber)
        /// </summary>
        public bool IsPublisher { get; set; }

        /// <summary>
        /// The data object with information about the connecting client (either a Publisher or Subscriber)
        /// </summary>
        public ClientInfo ClientInfo { get; set; }
    }
}
