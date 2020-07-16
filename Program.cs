using System;

namespace PublishSubscribeBroker
{
    // Main entry point to start the publish-subscribe broker test application
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Info("Starting Publish-Subscribe Broker test application...");

            // TODO
        }

        // Initialize a new server
        private static void StartBrokerServer(string ipAddress, int port)
        {
            try
            {
                // TODO

                Logger.Info("Broker server started");
            }
            catch (Exception e)
            {
                Logger.Error("Broker server failed to start" + Environment.NewLine + e.Message);
            }
        }

        // Initialize a new subscriber client
        private static void StartSubscriberClient(string ipAddress, int port)
        {
            try
            {
                // TODO

                Logger.Info("Subscriber client started");
            }
            catch (Exception e)
            {
                Logger.Error("Subscriber client failed to start" + Environment.NewLine + e.Message);
            }
        }

        // Initialize a new publisher client
        private static void StartPublisherClient(string ipAddress, int port)
        {
            try
            {
                // TODO

                Logger.Info("Publisher client started");
            }
            catch (Exception e)
            {
                Logger.Error("Publisher client failed to start" + Environment.NewLine + e.Message);
            }
        }
    }
}
