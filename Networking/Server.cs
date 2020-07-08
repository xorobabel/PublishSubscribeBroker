using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace PublishSubscribeBroker.Networking
{
    // Server class that can asynchronously connect with clients to send/receive messages
    class Server
    {
        private TcpListener listener; // The TCP listener used by the server for communication

        // Constructor to build a server with the specified IP and port
        public Server(string ipAddress, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ipAddress), port);
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
            AcceptConnection(); // Begin listening for another client

            // TODO
        }

    }
}
