using System;
using System.Collections.Generic;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Base class for server responses in a pub/sub system, where a response has a type to distinguish its contents
    /// </summary>
    [Serializable]
    public class Response
    {
        /// <summary>
        /// Enumerator for determining the type/operation of a response
        /// </summary>
        public enum ResponseType { INFO, NEW_MESSAGE, LIST_TOPICS }

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
    }

    /// <summary>
    /// Specialized response for when the server is sending a newly-published message from a subscribed topic
    /// </summary>
    /// <typeparam name="T">The type of the message contents (determined by protocol)</typeparam>
    [Serializable]
    public class NewMessageResponse<T> : Response
    {
        /// <summary>
        /// The topic from which the new message originated
        /// </summary>
        public Topic Topic { get; set; }

        /// <summary>
        /// The message/content that is being delivered to the client
        /// </summary>
        public Message<T> Message { get; set; }
    }

    /// <summary>
    /// Specialized response for when the server is sending a list of all active topics
    /// </summary>
    [Serializable]
    public class ListTopicsResponse : Response
    {
        /// <summary>
        /// The list of active topics available for the client to subscribe to
        /// </summary>
        public List<Topic> Topics { get; set; }
    }
}
