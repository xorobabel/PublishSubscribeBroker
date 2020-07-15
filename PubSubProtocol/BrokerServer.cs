using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
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
            // Attempt to receive a request from the client
            TryReceive(stream);

            // Attempt to send a response or new message to the client
            TrySend(clientId, stream);
        }

        /// <summary>
        /// Attempt to receive a request from the client if there is data to be read
        /// </summary>
        /// <param name="stream">The network stream used for communication with the client</param>
        protected void TryReceive(NetworkStream stream)
        {
            if (stream.DataAvailable)
            {
                // Receive and process a request object from the client
                Request request = ReceiveMessage<Request>(stream);
                ProcessRequest(request, stream);
            }
        }

        /// <summary>
        /// Process a request received from the client and take action based on the request type
        /// </summary>
        /// <param name="request">The received request to process</param>
        /// <param name="stream">The network stream used for communication with the client</param>
        protected void ProcessRequest(Request request, NetworkStream stream)
        {
            if (request.Type == RequestType.CREATE_TOPIC)
            {
                // Create a new topic and send back the created topic's name and assigned unique ID
                NameIdPair topicInfo = CreateTopic((request as CreateTopicRequest).TopicName);
                SendMessage(new TopicCreatedResponse(topicInfo), stream);
            }
            else if (request.Type == RequestType.LIST_TOPICS)
            {
                // Send back a list of active topics (without the topics' subscriber lists)
                List<NameIdPair> clientTopicList = new List<NameIdPair>();
                foreach (Topic topic in topics.Values)
                    clientTopicList.Add(topic.Info);
                SendMessage(new ListTopicsResponse(clientTopicList), stream);
            }
            else if (request.Type == RequestType.PUBLISH)
            {
                // TODO - publish a message to a topic
            }
            else if (request.Type == RequestType.SUBSCRIBE)
            {
                // TODO - subscribe the client to a topic
            }
            else if (request.Type == RequestType.UNSUBSCRIBE)
            {
                // TODO - unsubscribe the client from a topic
            }
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
                    {
                        SendMessage(response, stream);
                    }
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
            Guid id = new Guid();
            NameIdPair topicInfo = new NameIdPair(name, id);
            topics[id] = new Topic(topicInfo);
            return topicInfo;
        }

    }
}
