using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace PublishSubscribeBroker.Networking
{
    // Client class that can connect to a server to send/receive messages
    class Client
    {
        private TcpClient client;           // The underlying TCP client used to communicate with a server

        // Constructor to build a client and connect to the server at the specified address and port
        public Client(string ipAddress, int port)
        {
            client = new TcpClient(ipAddress, port);
            HandleCommunication();
        }

        // Disconnect from the currently-connected server (if connected)
        public void Disconnect()
        {
            client.Close();
        }

        // Check if the client is currently connected to the server
        public bool IsConnected()
        {
            return client.Connected;
        }

        // Defines the communication behavior of the client while connected to the server, using the client's communication stream
        protected virtual void HandleCommunication()
        {
            // Simple example handler: Get input from the user and send it immediately to the server
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
                    // Encode the message and send it to the server
                    Message message = new Message(Encoding.UTF8.GetBytes(input));
                    byte[] data = message.Encode();
                    clientStream.Write(data, 0, data.Length);
                }
            }
            clientStream.Dispose();
            // -----

            // NOTE: This function can be overridden by a child class to provide a more specific protocol
        }

    }
}
