using System;
using System.Collections.Concurrent;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Data object containing a topic's information and a thread-safe collection of its subscribers
    /// </summary>
    public class Topic
    {
        /// <summary>
        /// The name and ID information of the topic
        /// </summary>
        public NameIdPair Info { get; set; }

        /// <summary>
        /// A thread-safe collection of subscribers that are currently subscribed to the topic, organized by unique ID
        /// </summary>
        // Note: Unfortunately, the concurrent collections library does not have a thread-safe list implementation
        public ConcurrentDictionary<Guid, string> Subscribers { get; set; }

        /// <summary>
        /// Constructor to initialize a new topic and create its subscriber list
        /// </summary>
        public Topic(NameIdPair info)
        {
            Info = info;
            Subscribers = new ConcurrentDictionary<Guid, string>();
        }
    }
}
