using PublishSubscribeBroker.Networking;
using System;
using System.Net;

namespace PublishSubscribeBroker
{

    // Main entry point to start the publish-subscribe broker
    class Program
    {
        private static BrokerServer server;     // The active broker server

        static void Main(string[] args)
        {
            Logger.Info("Starting Publish-Subscribe Broker...");

            try
            {
                // Start the broker server
                StartBroker("127.0.0.1", 8008);

                Logger.Info("Broker server started");
            }
            catch (Exception e) {
                Logger.Error("Broker server failed to start" + Environment.NewLine + e.Message);
            }

            try
            {
                // Start a client
                Client client = new Client("127.0.0.1", 8008);

                Logger.Info("Client started");
            }
            catch (Exception e)
            {
                Logger.Error("Client failed to start" + Environment.NewLine + e.Message);
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
