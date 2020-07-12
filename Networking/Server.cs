using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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
    class Server
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
            AcceptClient();
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
        private void AcceptClient()
        {
            listener.BeginAcceptTcpClient(HandleConnection, listener);
        }

        /// <summary>
        /// Handle a new client connection
        /// </summary>
        private void HandleConnection(IAsyncResult result)
        {
            // Begin asynchronously listening for another client
            AcceptClient();

            // Accept the current connection and add it to list of clients
            TcpClient client = listener.EndAcceptTcpClient(result);
            Guid clientId = AddClient(client);

            // Handle communication behavior
            HandleCommunication(clientId);
            RemoveClient(clientId);
        }

        /// <summary>
        /// Assign the provided client a new ID and add it to the list of clients
        /// </summary>
        /// <param name="newClient">The new client needing to be added</param>
        /// <returns>The ID (as a Guid) of the newly-added client</returns>
        private Guid AddClient(TcpClient newClient)
        {
            Guid id = Guid.NewGuid();
            clients[id] = newClient;
            return id;
        }

        /// <summary>
        /// Performs the communication behavior of the server while the client with the specified ID is connected
        /// <br/><br/>
        /// This method can be overridden by a subclass to provide a custom communication protocol
        /// </summary>
        /// <param name="id">The ID of the connected client</param>
        protected virtual void HandleCommunication(Guid id)
        {
            // Simple example protocol: Print any messages received from the client
            // -----
            TcpClient client = clients[id];
            NetworkStream clientStream = client.GetStream();

            while (client.Connected)
            {
                // Extract length-of-message information
                int lengthBytesRead = 0;
                byte[] lengthBytes = new byte[4];
                while (lengthBytesRead < lengthBytes.Length)
                {
                    lengthBytesRead += clientStream.Read(lengthBytes, lengthBytesRead, lengthBytes.Length - lengthBytesRead);
                }
                int length = BitConverter.ToInt32(lengthBytes, 0);

                // Receive the main message contents
                int messageBytesRead = 0;
                byte[] messageBytes = new byte[length];
                while (messageBytesRead < messageBytes.Length)
                {
                    messageBytesRead += clientStream.Read(messageBytes, messageBytesRead, messageBytes.Length - messageBytesRead);
                }

                // Deserialize the received message object (a string in this protocol)
                string message;
                using (MemoryStream messageStream = new MemoryStream(messageBytes))
                {
                    message = new BinaryFormatter().Deserialize(messageStream) as string;
                }

                // Output the received message
                Console.WriteLine("Message received: {0}", message);
            }
        }

        /// <summary>
        /// Remove the client with the specified ID from the list of clients and close the connection to it
        /// </summary>
        /// <param name="id">The ID of the client to remove</param>
        private void RemoveClient(Guid id)
        {
            TcpClient client;
            bool wasRemoved = clients.TryRemove(id, out client);
            if (wasRemoved)
                client.Close();
        }

    }
}
