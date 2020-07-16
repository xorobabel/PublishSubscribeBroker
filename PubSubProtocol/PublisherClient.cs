using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using PublishSubscribeBroker.Networking;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Specialized client to act as a publisher in a publish-subscribe system
    /// </summary>
    class PublisherClient : Client
    {
        /// <summary>
        /// The readable name of the publisher client
        /// </summary>
        public string Name { get; set; } = "anonymous";

        /// <summary>
        /// The list of topics that the publisher "owns" (has created and can publish to)
        /// </summary>
        public List<NameIdPair> OwnedTopics { get; private set; }

        /// <summary>
        /// A thread-safe queue of pending requests from the user to be sent sequentially to the server
        /// </summary>
        // Note: Thread-safe to allow a separate thread to accept user input and add requests asynchronously
        private ConcurrentQueue<Request> pendingRequests;

        /// <summary>
        /// Whether the client is currently waiting for a response from the server or not
        /// </summary>
        private bool waitingForResponse = false;

        public PublisherClient(string ipAddress, int port) : base(ipAddress, port)
        {
            pendingRequests = new ConcurrentQueue<Request>();
            OwnedTopics = new List<NameIdPair>();
        }

        /// <summary>
        /// Override method to handle the publish-subscribe protocol on the publisher side
        /// </summary>
        /// <param name="stream">The network stream used for communication with the server</param>
        protected override void HandleProtocol(NetworkStream stream)
        {
            try
            {
                // Attempt to receive a response from the server
                TryReceive(stream);

                // Attempt to send a request to the server (if one has been initiated)
                TrySend(stream);

                // Wait a short time to help balance CPU usage
                Thread.Sleep(20);
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
            if (waitingForResponse)
            {
                // Handle responses to previous client requests
                if (response.Type == ResponseType.TOPIC_CREATED)
                {
                    // Add the new topic to the list of owned topics
                    NameIdPair topicInfo = (response as TopicCreatedResponse).TopicInfo;
                    OwnedTopics.Add(topicInfo);
                    Console.WriteLine("[Topic \"{0}\" has been created]", topicInfo.Name);
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
        /// Construct a new publish request to publish the provided message to the specified topic
        /// </summary>
        /// <param name="topicInfo">The name and unique ID of the topic to publish to</param>
        /// <param name="messageContent">The content of the message to publish</param>
        /// <returns>A new publish request to publish the provided message to the specified topic</returns>
        public PublishRequest<T> MakePublishRequest<T>(NameIdPair topicInfo, T messageContent)
        {
            Message<T> message = new Message<T>
            {
                Timestamp = DateTime.UtcNow,
                PublisherInfo = new NameIdPair(Name, ID),
                TopicInfo = topicInfo,
                Content = messageContent
            };
            return new PublishRequest<T>(message);
        }
    }
}
