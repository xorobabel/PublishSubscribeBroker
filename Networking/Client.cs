using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PublishSubscribeBroker.Networking
{
    /// <summary>
    /// Client class that can connect to a server to send and receive messages
    /// </summary>
    class Client
    {
        /// <summary>
        /// The underlying TCP client used to communicate with a server
        /// </summary>
        protected TcpClient client;

        /// <summary>
        /// Constructor to build a client and connect to the server at the specified address and port
        /// </summary>
        public Client(string ipAddress, int port)
        {
            client = new TcpClient(ipAddress, port);
        }

        /// <summary>
        /// Start asynchronously handling communication between the client and the server
        /// </summary>
        public void StartClient()
        {
            Task.Factory.StartNew(() => HandleCommunication());
        }

        /// <summary>
        /// Disconnect from the currently-connected server (if connected)
        /// </summary>
        public void Disconnect()
        {
            client.Close();
        }

        /// <summary>
        /// Check if the client is currently connected to a server
        /// </summary>
        /// <returns>Whether the client is connected or not</returns>
        public bool IsConnected()
        {
            return client.Connected;
        }

        /// <summary>
        /// Performs the communication behavior of the client while connected to a server
        /// <br/><br/>
        /// This method can be overridden by a subclass to provide a custom communication protocol
        /// </summary>
        protected virtual void HandleCommunication()
        {
            // Simple example protocol: Get input from the user and send it immediately to the server
            // -----
            NetworkStream clientStream = client.GetStream();

            while (IsConnected())
            {
                // Get a message from the user to send
                string input = Console.ReadLine();

                // Check for exit/disconnect command
                if (String.Equals(input.ToLower(), "exit") || String.Equals(input.ToLower(), "disconnect"))
                    Disconnect();
                else
                {
                    // Format the message and send it to the server
                    byte[] message = new SendableMessage<string>(input).Format();
                    clientStream.Write(message, 0, message.Length);
                }
            }
            // -----
        }

    }
}
