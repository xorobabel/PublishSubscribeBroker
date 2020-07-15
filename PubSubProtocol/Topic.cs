using System;
using System.Collections.Generic;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Data object containing information about a topic and its subscribers
    /// </summary>
    [Serializable]
    public class Topic
    {
        /// <summary>
        /// The name and ID information of the topic
        /// </summary>
        public NameIdPair Info { get; set; }

        /// <summary>
        /// The list of subscribers that are currently subscribed to the topic
        /// </summary>
        public List<NameIdPair> Subscribers { get; set; }

        /// <summary>
        /// Constructor to initialize a new topic and create its subscriber list
        /// </summary>
        public Topic(NameIdPair info)
        {
            Info = info;
            Subscribers = new List<NameIdPair>();
        }
    }
}
