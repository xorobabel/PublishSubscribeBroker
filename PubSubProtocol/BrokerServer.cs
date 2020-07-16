using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using PublishSubscribeBroker.Networking;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Specialized server class to act as a message broker in a publish-subscribe system
    /// </summary>
    class BrokerServer : Server
    {
        /// <summary>
        /// A thread-safe collection of all active topics, organized by the unique ID of each topic
        /// </summary>
        protected ConcurrentDictionary<Guid, Topic> topics;

        /// <summary>
        /// A thread-safe collection of client response queues, organized by the unique ID of each client
        /// </summary>
        protected ConcurrentDictionary<Guid, ConcurrentQueue<Response>> responseQueues;

        /// <summary>
        /// Constructor to build a publish-subscribe broker server with the specified IP and port
        /// </summary>
        public BrokerServer(string ipAddress, int port) : base(ipAddress, port)
        {
            topics = new ConcurrentDictionary<Guid, Topic>();
            responseQueues = new ConcurrentDictionary<Guid, ConcurrentQueue<Response>>();
        }

        /// <summary>
        /// Override method to handle the publish-subscribe protocol on the server/broker side for a client
        /// </summary>
        /// <param name="clientId">The unique ID of the connected client being handled</param>
        /// <param name="stream">The network stream used for communication with the client</param>
        protected override void HandleProtocol(Guid clientId, NetworkStream stream)
        {
            try
            {
                // Attempt to receive a request from the client
                TryReceive(clientId, stream);

                // Attempt to send a response or new message to the client
                TrySend(clientId, stream);

                // Wait a short time to help balance CPU usage
                Thread.Sleep(20);
            }
            catch (Exception e)
            {
                // An exception means the client is unexpectedly inaccessible
                Console.WriteLine("Warning: Connection to a client has been lost unexpectedly");
                Console.WriteLine("  " + e.Message);
                RemoveClient(clientId);
            }
        }

        /// <summary>
        /// Attempt to receive a request from the client if there is data to be read
        /// </summary>
        /// <param name="clientId">The unique ID of the connected client</param>
        /// <param name="stream">The network stream used for communication with the client</param>
        protected void TryReceive(Guid clientId, NetworkStream stream)
        {
            if (stream.DataAvailable)
            {
                // Receive and process a request object from the client
                Request request = ReceiveMessage<Request>(stream);
                Response response = ProcessRequest(request);

                // Send back an appropriate response
                AddResponse(clientId, response);
            }
        }

        /// <summary>
        /// Process a request received from the client and take action based on the request type
        /// </summary>
        /// <param name="request">The received request to process</param>
        /// <returns>An appropriate response to the client's request</returns>
        protected Response ProcessRequest(Request request)
        {
            Response response = new InfoResponse("Error: Unknown request");

            if (request.Type == RequestType.CREATE_TOPIC)
            {
                // Create a new topic and send back the created topic's name and assigned unique ID
                NameIdPair topicInfo = CreateTopic((request as CreateTopicRequest).TopicName);
                response = new TopicCreatedResponse(topicInfo);
            }
            else if (request.Type == RequestType.LIST_TOPICS)
            {
                // Send back a list of active topics (without the topics' subscriber lists)
                response = new ListTopicsResponse(GetClientTopicList());
            }
            else if (request.Type == RequestType.PUBLISH)
            {
                // Try to publish the received message
                Message<string> message = (request as PublishRequest<string>).Message;
                if (PublishMessage(message))
                    response = new InfoResponse("Message published to topic \"" + message.TopicInfo.Name + "\"");
                else
                    // Send back an error message if the request was for a nonexistent topic
                    response = new InfoResponse("Message could not be published (Topic \"" + message.TopicInfo.Name + "\" does not exist)");
            }
            else if (request.Type == RequestType.SUBSCRIBE)
            {
                // Try to subscribe the client to the requested topic
                if (Subscribe(request as SubscribeRequest))
                    response = new InfoResponse("Subscription added");
                else
                    // Send back an error message if the request was for a nonexistent topic
                    response = new InfoResponse("Unable to add subscription (Topic does not exist)");
            }
            else if (request.Type == RequestType.UNSUBSCRIBE)
            {
                // Try to unsubscribe the client from the requested topic
                if (Unsubscribe(request as UnsubscribeRequest))
                    response = new InfoResponse("Subscription removed");
                else
                    // Send back an error message if the subscription couldn't be removed
                    response = new InfoResponse("Unable to remove subscription (Subscription may not exist)");
            }
            return response;
        }

        /// <summary>
        /// Attempt to send a response to the client if there is a pending response to send
        /// </summary>
        /// <param name="clientId">The unique ID of the client to try sending a response to</param>
        /// <param name="stream">The network stream used for communication with the client</param>
        protected void TrySend(Guid clientId, NetworkStream stream)
        {
            // Check that a response queue exists for the client before trying to retrieve from it
            if (responseQueues.ContainsKey(clientId))
            {
                ConcurrentQueue<Response> queue = responseQueues[clientId];
                if (queue.Count > 0)
                {
                    // Send the pending response to the client
                    if (queue.TryDequeue(out Response response))
                        SendMessage(response, stream);
                }
            }
        }

        /// <summary>
        /// Add a new pending response to send to the client with the specified ID
        /// </summary>
        /// <param name="clientId">The unique ID of the client to send a response to</param>
        /// <param name="request">The response object to send</param>
        public void AddResponse(Guid clientId, Response response)
        {
            // Ensure the specified client has a response queue to add to
            if (!responseQueues.ContainsKey(clientId))
                responseQueues[clientId] = new ConcurrentQueue<Response>();

            // Add the response to the client's queue
            responseQueues[clientId].Enqueue(response);
        }

        /// <summary>
        /// Create a new topic with the specified name and add it to the collection of active topics
        /// </summary>
        /// <param name="name">The desired name of the new topic</param>
        /// <returns>The name and assigned unique ID of the new topic</returns>
        protected NameIdPair CreateTopic(string name)
        {
            Guid id = Guid.NewGuid();
            NameIdPair topicInfo = new NameIdPair(name, id);
            topics[id] = new Topic(topicInfo);
            return topicInfo;
        }

        /// <summary>
        /// Get a client-safe list of topics (without each topic's list of subscribers)
        /// </summary>
        /// <returns>A list with the names and unique IDs of all active topics</returns>
        protected List<NameIdPair> GetClientTopicList()
        {
            List<NameIdPair> clientTopicList = new List<NameIdPair>();
            foreach (Topic topic in topics.Values)
                clientTopicList.Add(topic.Info);
            return clientTopicList;
        }

        /// <summary>
        /// Attempt to publish the provided message to all subscribers of the message's topic
        /// </summary>
        /// <typeparam name="T">The type of the message's contents</typeparam>
        /// <param name="message">The message object containing information about the message and the message itself</param>
        /// <returns>Whether the message was successfully published or not</returns>
        protected bool PublishMessage<T>(Message<T> message)
        {
            bool success;
            // Ensure the topic exists before publishing
            if (topics.ContainsKey(message.TopicInfo.ID))
            {
                Topic topic = topics[message.TopicInfo.ID];

                // Send the message to the topic's subscribers by adding a response to their individual response queues
                foreach (Guid subscriberId in topic.Subscribers.Keys)
                    AddResponse(subscriberId, new NewMessageResponse<T>(message));

                success = true;
            }
            else
                success = false;
            return success;
        }

        /// <summary>
        /// Attempt to subscribe a client to a topic using the information in the provided SubscribeRequest
        /// </summary>
        /// <param name="request">The request object containing information for the subscription action</param>
        /// <returns>Whether the client was successfully subscribed to the topic or not</returns>
        protected bool Subscribe(SubscribeRequest request)
        {
            bool success;
            if (topics.ContainsKey(request.TopicID))
            {
                // Add the subscriber to the topic's list of subscribers
                topics[request.TopicID].Subscribers[request.Subscriber.ID] = request.Subscriber.Name;

                success = true;
            }
            else
                success = false;
            return success;
        }

        /// <summary>
        /// Attempt to unsubscribe a client from a topic using the information in the provided UnsubscribeRequest
        /// </summary>
        /// <param name="request">The request object containing information for the unsubscription action</param>
        /// <returns>Whether the client was successfully unsubscribed from the topic or not</returns>
        protected bool Unsubscribe(UnsubscribeRequest request)
        {
            bool success;
            if (topics.ContainsKey(request.TopicID))
            {
                // Try to remove the subscriber from the topic's list of subscribers
                success = topics[request.TopicID].Subscribers.TryRemove(request.Subscriber.ID, out string name);
            }
            else
                success = false;
            return success;
        }

    }
}
