using System;
using System.Collections.Generic;
using System.Text;

namespace PublishSubscribeBroker.Networking
{
    // Client class that can connect to a server to send/receive messages
    class Client
    {
        
        // Attempt to establish a connection to the server at the specifified IP and port
        // -- Returns true if connected successfully
        public bool Connect(string ipAddress, string port)
        {
            bool success = true;

            // TODO

            return success;
        }

        // Disconnect from the currently-connected server (if connected)
        public void Disconnect()
        {
            // TODO
        }

        // Check if the client is currently connected to a server
        public bool IsConnected()
        {
            return false; // TODO
        }

    }
}
