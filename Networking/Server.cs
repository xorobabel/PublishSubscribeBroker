using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace PublishSubscribeBroker.Networking
{
    /// <summary>
    /// Extendable data object containing information about a connected client
    /// </summary>
    [Serializable]
    public class ClientInfo
    {
        /// <summary>
        /// The unique internal ID of the client
        /// </summary>
        public Guid ID { get; set; }
    }

    /// <summary>
    /// Server class that can asynchronously connect with multiple clients to send and receive messages
    /// </summary>
    class Server : Communicator
    {
        /// <summary>
        /// The TCP listener used by the server for communication
        /// </summary>
        protected TcpListener listener;

        /// <summary>
        /// A thread-safe collection of all clients currently connected to the server, organized by unique ID
        /// </summary>
        protected ConcurrentDictionary<Guid, TcpClient> clients;

        /// <summary>
        /// Whether the server is currently running or not
        /// </summary>
        protected bool running;

        /// <summary>
        /// Constructor to build a server with the specified IP address and port
        /// </summary>
        public Server(string ipAddress, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            clients = new ConcurrentDictionary<Guid, TcpClient>();
        }

        /// <summary>
        /// Start the server and begin listening for clients
        /// </summary>
        public void StartServer()
        {
            listener.Start();
            ListenForClient();
            running = true;
        }

        /// <summary>
        /// Stop the server and close the connection to all clients
        /// </summary>
        public void StopServer()
        {
            listener.Stop();
            foreach (TcpClient client in clients.Values)
            {
                client.Close();
            }
            running = false;
        }

        /// <summary>
        /// Check if the server is currently running
        /// </summary>
        /// <returns>Whether the server is running or not</returns>
        public bool IsRunning()
        {
            return running;
        }

        /// <summary>
        /// Asynchronously listen for and accept a new client connection
        /// </summary>
        protected void ListenForClient()
        {
            listener.BeginAcceptTcpClient(AcceptClient, listener);
        }

        /// <summary>
        /// Accept and handle a new client connection
        /// </summary>
        protected void AcceptClient(IAsyncResult result)
        {
            // Begin asynchronously listening for another client
            ListenForClient();

            // Accept the current connection and add it to list of clients
            TcpClient client = listener.EndAcceptTcpClient(result);
            Guid clientId = AddClient(client);

            // Handle communication behavior
            HandleCommunication(clientId);

            // Remove the client when done
            RemoveClient(clientId);
        }

        /// <summary>
        /// Assign the provided client a new unique ID and add it to the list of clients
        /// </summary>
        /// <param name="newClient">The new client needing to be added</param>
        /// <returns>The unique ID (as a Guid) of the newly-added client</returns>
        protected Guid AddClient(TcpClient newClient)
        {
            Guid id = Guid.NewGuid();
            clients[id] = newClient;
            return id;
        }

        /// <summary>
        /// Remove the client with the specified ID from the list of clients and close the connection to it
        /// </summary>
        /// <param name="id">The ID of the client to remove</param>
        protected void RemoveClient(Guid id)
        {
            TcpClient client;
            bool wasRemoved = clients.TryRemove(id, out client);
            if (wasRemoved)
                client.Close();
        }

        /// <summary>
        /// Performs the communication behavior of the server while the client with the specified ID is connected
        /// <br/><br/>
        /// Acts as a wrapper for the protocol defined in HandleProtocol()
        /// </summary>
        /// <param name="id">The unique ID of the connected client</param>
        protected virtual void HandleCommunication(Guid id)
        {
            // Get the client with the specified unique ID
            TcpClient client = clients[id];
            NetworkStream clientStream = client.GetStream();

            while (client.Connected)
            {
                // Use a separate protocol handler function to provide more granularity and control for subclasses
                HandleProtocol(clientStream);
            }
        }

        /// <summary>
        /// Handles the specific protocol used for communication with the client over the provided network stream
        /// <br/><br/>
        /// This method can be overridden by a subclass to provide a custom communication protocol
        /// </summary>
        /// <param name="stream">The stream used for communication with the client</param>
        protected virtual void HandleProtocol(NetworkStream stream)
        {
            // Simple example protocol: Print any messages received from the client
            // ------

            // Get a string message sent by the user
            string message = ReceiveMessage<string>(stream);

            // Output the received message
            Console.WriteLine("[SERVER] Message received: {0}", message);
        }

    }
}
