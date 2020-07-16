using PublishSubscribeBroker.Networking;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PublishSubscribeBroker
{
    // Main entry point to start a test of the Networking package (simple message-sending protocol)
    class NetworkingTest
    {
        private static Server server;           // The active server
        private static List<Client> clients;    // The active clients

        static void Main(string[] args)
        {
            clients = new List<Client>();

            Logger.Info("Starting Networking package test application...");

            // Start the server
            Task serverTask = Task.Factory.StartNew(() => StartServer("127.0.0.1", 8008));
            serverTask.Wait();

            // Start a client
            StartClient("localhost", 8008);

            // Wait for the server to shut down or all clients to disconnect
            bool clientStillRunning;
            while (server.IsRunning())
            {
                Thread.Sleep(1000); // Check every 1 second

                // Check connected status of all active clients
                clientStillRunning = false;
                foreach (Client client in clients)
                    if (client.IsConnected())
                        clientStillRunning = true;

                if (!clientStillRunning)
                {
                    server.StopServer();
                    Logger.Info("Server stopped because all clients disconnected");
                }
            }
        }

        // Initialize a new server
        private static void StartServer(string ipAddress, int port)
        {
            try
            {
                server = new Server(ipAddress, port);
                server.StartServer();

                Logger.Info("Server started");
            }
            catch (Exception e)
            {
                Logger.Error("Server failed to start" + Environment.NewLine + e.Message);
            }
        }

        // Initialize a new client
        private static void StartClient(string ipAddress, int port)
        {
            try
            {
                Client client = new Client(ipAddress, port);
                clients.Add(client);
                client.StartClient();

                Logger.Info("Client started");
            }
            catch (Exception e)
            {
                Logger.Error("Client failed to start" + Environment.NewLine + e.Message);
            }
        }
    }
}
