using System;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Enumerator for determining the type/operation of a request
    /// </summary>
    public enum RequestType { SUBSCRIBE, UNSUBSCRIBE, PUBLISH, CREATE_TOPIC, LIST_TOPICS }

    /// <summary>
    /// Base class for client requests in a pub/sub system, where a request has a type to distinguish its contents
    /// </summary>
    [Serializable]
    public class Request
    {
        /// <summary>
        /// The type/operation of this request
        /// </summary>
        public RequestType Type { get; set; }
    }

    /// <summary>
    /// Specialized request for a subscription request, where a client subscribes to a topic
    /// </summary>
    [Serializable]
    public class SubscribeRequest : Request
    {
        /// <summary>
        /// The ID of the topic to which the client wants to subscribe
        /// </summary>
        public Guid TopicID { get; set; }

        /// <summary>
        /// Construct a SubscribeRequest for the specified topic
        /// </summary>
        /// <param name="topicId">The ID of the topic to subscribe to</param>
        public SubscribeRequest(Guid topicId)
        {
            Type = RequestType.SUBSCRIBE;
            TopicID = topicId;
        }
    }

    /// <summary>
    /// Specialized request for an unsubscription request, where a client unsubscribes from a topic
    /// </summary>
    [Serializable]
    public class UnsubscribeRequest : Request
    {
        /// <summary>
        /// The ID of the topic to which the client wants to unsubscribe
        /// </summary>
        public Guid TopicID { get; set; }

        /// <summary>
        /// Construct an UnsubscribeRequest for the specified topic
        /// </summary>
        /// <param name="topicId">The ID of the topic to unsubscribe from</param>
        public UnsubscribeRequest(Guid topicId)
        {
            Type = RequestType.UNSUBSCRIBE;
            TopicID = topicId;
        }
    }

    /// <summary>
    /// Specialized request for a publish request, where a client publishes a message to a topic
    /// </summary>
    /// <typeparam name="T">The type of the message contents (determined by protocol)</typeparam>
    [Serializable]
    public class PublishRequest<T> : Request
    {
        /// <summary>
        /// The ID of the topic to which the client wants to publish
        /// </summary>
        public Guid TopicID { get; set; }

        /// <summary>
        /// The message/content that the client wants to publish
        /// </summary>
        public Message<T> Message { get; set; }

        /// <summary>
        /// Construct a PublishRequest for the specified topic and message
        /// </summary>
        /// <param name="topicId">The ID of the topic to publish to</param>
        /// <param name="message">The message to publish</param>
        public PublishRequest(Guid topicId, Message<T> message)
        {
            Type = RequestType.PUBLISH;
            TopicID = topicId;
            Message = message;
        }
    }

    /// <summary>
    /// Specialized request for a topic creation request, where a client creates a new topic
    /// </summary>
    [Serializable]
    public class CreateTopicRequest : Request
    {
        /// <summary>
        /// The name of the new topic that the client wishes to create (an ID will be assigned by the server)
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// Construct a CreateTopicRequest to make a topic with the specified name
        /// </summary>
        /// <param name="name">The desired name of the new topic</param>
        public CreateTopicRequest(string name)
        {
            Type = RequestType.CREATE_TOPIC;
            TopicName = name;
        }
    }

    /// <summary>
    /// Specialized request for a topic listing request, where a client requests a list of active topics
    /// </summary>
    public class ListTopicsRequest : Request
    {
        /// <summary>
        /// Construct a ListTopicsRequest to get a list of active topics
        /// </summary>
        public ListTopicsRequest()
        {
            Type = RequestType.LIST_TOPICS;
        }
    }
}
