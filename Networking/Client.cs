using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PublishSubscribeBroker.Networking
{
    /// <summary>
    /// Client class that can connect to a server to send and receive messages
    /// </summary>
    class Client : Communicator
    {
        /// <summary>
        /// The underlying TCP client used to communicate with a server
        /// </summary>
        protected TcpClient client;

        /// <summary>
        /// The unique ID of this client, assigned by the server upon connection
        /// </summary>
        public Guid ID { get; set; }

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
            Task.Factory.StartNew(() => BeginCommunication());
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
        /// Starts the communication behavior of the client while connected to a server
        /// <br/><br/>
        /// Acts as a wrapper for the protocol defined in HandleProtocol()
        /// </summary>
        protected virtual void BeginCommunication()
        {
            // Get the stream used for network communication
            NetworkStream clientStream = client.GetStream();

            // Receive the client's assigned unique ID from the server
            ID = ReceiveMessage<Guid>(clientStream);

            while (IsConnected())
            {
                // Use a separate protocol handler function to provide more granularity and control for subclasses
                HandleProtocol(clientStream);
            }
        }

        /// <summary>
        /// Handles the specific protocol used for communication with the server over the provided network stream
        /// <br/><br/>
        /// This method can be overridden by a subclass to provide a custom communication protocol
        /// </summary>
        /// <param name="stream">The stream used for communication with the server</param>
        protected virtual void HandleProtocol(NetworkStream stream)
        {
            // Simple example protocol: Get input from the user and send it immediately to the server
            // ------

            // Get a message from the user to send
            string input = Console.ReadLine();

            // Check for exit/disconnect command
            if (string.Equals(input.ToLower(), "exit") || string.Equals(input.ToLower(), "disconnect"))
                Disconnect();
            else
            {
                // Send the message to the server
                SendMessage(input, stream);

                Console.WriteLine("[CLIENT] Message sent");
            }
        }

    }
}
