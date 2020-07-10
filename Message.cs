using System;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Serializable message object containing information for a published message in a pub/sub system
    /// </summary>
    [Serializable]
    public class Message<T>
    {
        /// <summary>
        /// The name or ID of the message publisher
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// The topic where the message was published
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// The date and time when the message was published
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The content of the message
        /// </summary>
        public T Content { get; set; }

    }
}
