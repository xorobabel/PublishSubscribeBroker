using System;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Base class for client requests in a pub/sub system, where a request has a type to distinguish its contents
    /// </summary>
    [Serializable]
    public class Request
    {
        /// <summary>
        /// Enumerator for determining the type/operation of a request
        /// </summary>
        public enum RequestType { SUBSCRIBE, UNSUBSCRIBE, PUBLISH, CREATE_TOPIC, LIST_TOPICS }

        /// <summary>
        /// The type/operation of this request
        /// </summary>
        public RequestType Type { get; set; }
    }

    /// <summary>
    /// Specialized request for a subscription request, where a client subscribes or unsubscribes to a topic
    /// </summary>
    [Serializable]
    public class SubscriptionRequest : Request
    {
        /// <summary>
        /// The topic to which the client wants to subscribe or unsubscribe
        /// </summary>
        public Topic Topic { get; set; }
    }

    /// <summary>
    /// Specialized request for a publish request, where a client publishes a message to a topic
    /// </summary>
    /// <typeparam name="T">The type of the message contents (determined by protocol)</typeparam>
    [Serializable]
    public class PublishRequest<T> : Request
    {
        /// <summary>
        /// The topic to which the client wants to publish
        /// </summary>
        public Topic Topic { get; set; }

        /// <summary>
        /// The message/content that the client wants to publish
        /// </summary>
        public Message<T> Message { get; set; }
    }

    /// <summary>
    /// Specialized request for a topic creation request, where a client creates a new topic
    /// </summary>
    [Serializable]
    public class CreateTopicRequest : Request
    {
        /// <summary>
        /// The new topic that the client wishes to create
        /// </summary>
        public Topic Topic { get; set; }
    }
}
