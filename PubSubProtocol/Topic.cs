using System;
using System.Collections.Generic;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Data object containing information about a topic
    /// </summary>
    [Serializable]
    public class Topic
    {
        /// <summary>
        /// The readable name of the topic
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique internal ID of the topic
        /// </summary>
        public Guid ID { get; set; }
    }

    /// <summary>
    /// Specialized Topic for the broker that also contains information about current subscribers
    /// </summary>
    public class BrokerTopic : Topic
    {
        /// <summary>
        /// The list of subscribers that are currently subscribed to this topic
        /// </summary>
        public List<Subscriber> Subscribers;
    }
}
