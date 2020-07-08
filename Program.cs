using System;
using System.Net;

namespace PublishSubscribeBroker
{

    // Main entry point to start the publish-subscribe broker
    class Program
    {
        private static BrokerServer server; // The active broker server

        static void Main(string[] args)
        {
            Logger.Info("Starting Publish-Subscribe Broker...");

            // Start the broker server
            StartBroker("127.0.0.1", 8008);

            if (server.IsStarted())
            {
                Logger.Info("Broker server started");

                // TODO
            }
            else
            {
                Logger.Error("Broker server failed to start");
            }

            // TODO
        }

        // Initialize a new broker server
        private static void StartBroker(string ipAddress, int port)
        {
            server = new BrokerServer(ipAddress, port);
            server.StartServer();
        }
    }
}
