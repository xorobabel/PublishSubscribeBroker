using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
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

    // Server class that can asynchronously connect with clients to send/receive messages
    class Server
    {
        /// <summary>
        /// The TCP listener used by the server for communication
        /// </summary>
        private TcpListener listener;
        /// <summary>
        /// A thread-safe collection of all clients currently connected to the server, organized by unique ID
        /// </summary>
        private ConcurrentDictionary<Guid, TcpClient> clients;

        // Constructor to build a server with the specified IP and port
        public Server(string ipAddress, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            clients = new ConcurrentDictionary<Guid, TcpClient>();
        }

        // Start the server and listen for clients
        public void StartServer()
        {
            listener.Start();
            AcceptConnection();
        }

        // Listen for and accept a new client connection
        private void AcceptConnection()
        {
            listener.BeginAcceptTcpClient(HandleConnection, listener);
        }

        // Handle a new client connection
        private void HandleConnection(IAsyncResult result)
        {
            // Begin listening for another client
            AcceptConnection();

            // Accept the current connection and add it to list of clients
            TcpClient client = listener.EndAcceptTcpClient(result);
            clients.Add(client);

            // Handle communication behavior
            HandleCommunication(client);
            clients.Remove(client);
        }

        // Defines the communication behavior of the server while a client is connected, using the client's communication stream
        protected virtual void HandleCommunication(TcpClient client)
        {
            // Simple example handler: Print any messages received from the client
            // -----
            NetworkStream clientStream = client.GetStream();
            MemoryStream dataStream = new MemoryStream();
            bool incompleteMessage = false;
            int length = -1;
            while (client.Connected)
            {
                // Monitor for new data to read
                if (clientStream.DataAvailable)
                {
                    byte[] data = new byte[1024];
                    int bytesRead = clientStream.Read(data, 0, data.Length);
                    dataStream.Write(data, 0, bytesRead);
                }

                // Handle received data
                if (incompleteMessage)
                {
                    // If enough data is available, get the message and output it
                    if (dataStream.Length >= length)
                    {
                        byte[] messageBytes = new byte[length];
                        dataStream.Read(messageBytes, 0, length);
                        string message = Encoding.UTF8.GetString(messageBytes);
                        Console.WriteLine("[CLIENT] " + message);
                        incompleteMessage = false;
                    }
                }
                else if (dataStream.Length > 4)
                {
                    // Extract length-of-message information
                    byte[] lengthBytes = new byte[4];
                    dataStream.Read(lengthBytes, 0, 4);
                    length = BitConverter.ToInt32(lengthBytes, 0);
                    Console.WriteLine("DEBUG: length = " + length);
                    incompleteMessage = true;
                }
            }
            clientStream.Dispose();
            dataStream.Dispose();
            // -----

            // NOTE: This function can be overridden by a child class to provide a more specific protocol
        }

    }
}
