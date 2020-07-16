using System;
using System.Collections.Generic;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Enumerator for determining the type/operation of a response
    /// </summary>
    public enum ResponseType { INFO, NEW_MESSAGE, TOPIC_CREATED, LIST_TOPICS }

    /// <summary>
    /// Base class for server responses in a pub/sub system, where a response has a type to distinguish its contents
    /// </summary>
    [Serializable]
    public class Response
    {
        /// <summary>
        /// The type/operation of this response
        /// </summary>
        public ResponseType Type { get; set; }
    }

    /// <summary>
    /// Specialized response for when the server is sending simple text information
    /// </summary>
    [Serializable]
    public class InfoResponse : Response
    {
        /// <summary>
        /// The text information sent in the response
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Construct an InfoResponse with the specified text
        /// </summary>
        /// <param name="text">The text content of the information response</param>
        public InfoResponse(string text)
        {
            Type = ResponseType.INFO;
            Text = text;
        }
    }

    /// <summary>
    /// Specialized response for when the server is sending a newly-published message from a subscribed topic
    /// </summary>
    /// <typeparam name="T">The type of the message contents (determined by protocol)</typeparam>
    [Serializable]
    public class NewMessageResponse<T> : Response
    {
        /// <summary>
        /// The message/content that is being delivered to the client
        /// </summary>
        public Message<T> Message { get; set; }

        /// <summary>
        /// Construct a NewMessageResponse with the specified message
        /// </summary>
        /// <param name="message">The newly-published message to send</param>
        public NewMessageResponse(Message<T> message)
        {
            Type = ResponseType.NEW_MESSAGE;
            Message = message;
        }
    }

    /// <summary>
    /// Specialized response for when the server has created a new topic at the client's request
    /// </summary>
    [Serializable]
    public class TopicCreatedResponse : Response
    {
        /// <summary>
        /// The name and assigned unique ID of the newly-created topic
        /// </summary>
        public NameIdPair TopicInfo { get; set; }

        /// <summary>
        /// Construct a TopicCreatedResponse with the new topic's information
        /// </summary>
        /// <param name="topicInfo">The name and assigned unique ID of the created topic</param>
        public TopicCreatedResponse(NameIdPair topicInfo)
        {
            Type = ResponseType.TOPIC_CREATED;
            TopicInfo = topicInfo;
        }
    }

    /// <summary>
    /// Specialized response for when the server is sending a list of all active topics
    /// </summary>
    [Serializable]
    public class ListTopicsResponse : Response
    {
        /// <summary>
        /// The list of active topics available for the client to subscribe to (name and ID)
        /// </summary>
        public List<NameIdPair> Topics { get; set; }

        /// <summary>
        /// Construct a ListTopicsResponse with the specified topic list
        /// </summary>
        /// <param name="topicList">The list of active topics</param>
        public ListTopicsResponse(List<NameIdPair> topicList)
        {
            Type = ResponseType.LIST_TOPICS;
            Topics = topicList;
        }
    }
}
