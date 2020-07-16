using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using PublishSubscribeBroker.Networking;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Specialized client to act as a subscriber in a publish-subscribe system
    /// </summary>
    class SubscriberClient : Client
    {
        /// <summary>
        /// The readable name of the subscriber client
        /// </summary>
        public string Name { get; set; } = "anonymous";

        /// <summary>
        /// A client-side cached list of topics available on the broker server
        /// </summary>
        public List<NameIdPair> TopicCache { get; private set; }

        /// <summary>
        /// A thread-safe queue of pending requests from the user to be sent sequentially to the server
        /// </summary>
        // Note: Thread-safe to allow a separate thread to accept user input and add requests asynchronously
        private ConcurrentQueue<Request> pendingRequests;

        /// <summary>
        /// Whether the client is currently waiting for a response from the server or not
        /// </summary>
        private bool waitingForResponse = false;

        public SubscriberClient(string ipAddress, int port) : base(ipAddress, port)
        {
            pendingRequests = new ConcurrentQueue<Request>();
        }

        /// <summary>
        /// Override method to handle the publish-subscribe protocol on the subscriber side
        /// </summary>
        /// <param name="stream">The network stream used for communication with the server</param>
        protected override void HandleProtocol(NetworkStream stream)
        {
            try
            {
                // Attempt to receive a message or response from the server
                TryReceive(stream);

                // Attempt to send a request to the server (if one has been initiated)
                TrySend(stream);
            }
            catch (Exception e)
            {
                // An exception means the server is unexpectedly inaccessible
                Console.WriteLine("Error: Connection to the server has been lost unexpectedly");
                Console.WriteLine("  " + e.Message);
                Disconnect();
            }
        }

        /// <summary>
        /// Attempt to receive data from the server if there is data to be read
        /// </summary>
        /// <param name="stream">The network stream used for communication with the server</param>
        protected void TryReceive(NetworkStream stream)
        {
            if (stream.DataAvailable)
            {
                // Receive and process a response object from the server
                Response response = ReceiveMessage<Response>(stream);
                ProcessResponse(response);
            }
        }

        /// <summary>
        /// Process a response received from the server and take action based on the response type
        /// </summary>
        /// <param name="response">The received response to process</param>
        protected void ProcessResponse(Response response)
        {
            if (response.Type == ResponseType.NEW_MESSAGE)
            {
                // Show a newly published message from the broker
                Message<string> message = (response as NewMessageResponse<string>).Message;

                Console.WriteLine("[New Message from \"{0}\" in topic \"{1}\"]" + Environment.NewLine + "{2}",
                    message.PublisherInfo.Name, message.TopicInfo.Name, message.Content);
            }
            else if (waitingForResponse)
            {
                // Handle responses to previous client requests
                if (response.Type == ResponseType.LIST_TOPICS)
                {
                    // Cache the topic list and show the list of topics
                    TopicCache = (response as ListTopicsResponse).Topics;
                    Console.WriteLine("[Topic List]");
                    int index = 0;
                    foreach (NameIdPair topic in TopicCache)
                    {
                        Console.WriteLine(" {0}. {1}", index, topic.Name);
                        index++;
                    }
                }
                else if (response.Type == ResponseType.INFO)
                {
                    // Show received information response
                    Console.WriteLine("[Info] {0}", (response as InfoResponse).Text);
                }
                waitingForResponse = false;
            }
        }

        /// <summary>
        /// Attempt to send a request to the server if there is a pending request to send
        /// </summary>
        /// <param name="stream">The network stream used for communication with the server</param>
        protected void TrySend(NetworkStream stream)
        {
            if (!waitingForResponse && pendingRequests.Count > 0)
            {
                // Send the pending request to the server
                if (pendingRequests.TryDequeue(out Request request))
                {
                    SendMessage(request, stream);
                    waitingForResponse = true;
                }
            }
        }

        /// <summary>
        /// Add a new pending request to send to the server
        /// </summary>
        /// <param name="request">The request object to send</param>
        public void AddRequest(Request request)
        {
            pendingRequests.Enqueue(request);
        }

        /// <summary>
        /// Construct a new subscription request for this subscriber client to subscribe to the topic with the provided ID
        /// </summary>
        /// <param name="topicId">The unique ID of the topic to subscribe to</param>
        /// <returns>A new subscription request for this subscriber client to subscribe to the specified topic</returns>
        public SubscribeRequest MakeSubscriptionRequest(Guid topicId)
        {
            return new SubscribeRequest(new NameIdPair(Name, ID), topicId);
        }

        /// <summary>
        /// Construct a new unsubscription request for this subscriber client to unsubscribe from the topic with the provided ID
        /// </summary>
        /// <param name="topicId">The unique ID of the topic to unsubscribe from</param>
        /// <returns>A new unsubscription request for this subscriber client to unsubscribe from the specified topic</returns>
        public UnsubscribeRequest MakeUnsubscriptionRequest(Guid topicId)
        {
            return new UnsubscribeRequest(new NameIdPair(Name, ID), topicId);
        }

        /// <summary>
        /// Searches for the topic with the specified readable name in the cached topic list and returns its unique ID
        /// </summary>
        /// <param name="name">The readable name of the topic to search for</param>
        /// <returns>The unique ID of the topic with the specified name, or Guid.Empty if it was not found</returns>
        public Guid FindTopicID(string name)
        {
            Guid found = Guid.Empty;
            int index = 0;
            while (found == Guid.Empty && index < TopicCache.Count)
            {
                if (TopicCache[index].Name == name)
                    found = TopicCache[index].ID;
                index++;
            }
            return found;
        }
    }
}
