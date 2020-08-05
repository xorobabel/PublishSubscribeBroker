using System;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Data object containing information for a published message in a pub/sub system
    /// </summary>
    [Serializable]
    public class Message<T> // Good class, clean and generic
    {
        /// <summary>
        /// The publisher of the message
        /// </summary>
        public NameIdPair PublisherInfo { get; set; }

        /// <summary>
        /// The name and unique ID of the topic the message was published to
        /// </summary>
        public NameIdPair TopicInfo { get; set; }

        /// <summary>
        /// The date and time when the message was published
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The content of the message (must be serializable for sending)
        /// </summary>
        public T Content { get; set; }
    }
}
