using System;
using System.Diagnostics;
using System.Threading;

namespace PublishSubscribeBroker
{
    // Main entry point to start the publish-subscribe broker test application
    class Program
    {
        public const int PORT = 8008;

        static void Main(string[] args)
        {
            Logger.Info("Starting Publish-Subscribe Broker test application...");

            if (args.Length > 1)
            {
                if (args[0].ToLower() == "-subscriber")
                {
                    // Start a subscriber client with a list of topic names as arguments
                    Logger.Info("-- Subscriber Client Instance --");
                    string[] subscriberArgs = new string[args.Length - 1];
                    for (int i = 0; i < args.Length - 1; i++)
                        subscriberArgs[i] = args[i + 1];
                    StartSubscriberClient("localhost", PORT, subscriberArgs);
                }
                else if (args[0].ToLower() == "-publisher")
                {
                    // Start a publisher client with a topic name as an argument
                    Logger.Info("-- Publisher Client Instance --");
                    StartPublisherClient("localhost", PORT, args[1]);
                }
            }
            else if (args.Length == 1)
            {
                // Show some help information
                Logger.Error("Invalid arguments!" + Environment.NewLine + "  Start without arguments to run the full test application with a broker server" +
                    Environment.NewLine + "  Use \"-publisher <topic name>\" to create a publisher client for the specified topic" +
                    Environment.NewLine + "  Use \"-subscriber <topic name> ... <topic name>\" to create a subscriber client and subscribe to the specified topics");
            }
            else
            {
                // No args: Start a broker server and some test clients
                Logger.Info("-- Broker Server Instance --");
                BrokerServer server = StartBrokerServer("127.0.0.1", PORT);

                // Create separate app instances to allow separate console output windows
                Logger.Info("Starting new application instances for clients...");
                StartNewAppInstance("-publisher test-topic-1");
                StartNewAppInstance("-publisher test-topic-2");
                StartNewAppInstance("-publisher test-topic-3");
                StartNewAppInstance("-subscriber test-topic-1 test-topic-2");
                StartNewAppInstance("-subscriber test-topic-2 test-topic-3");

                // Keep process alive while server is running
                while (server.IsRunning())
                {
                    Thread.Sleep(500);
                }
                Logger.Warn("Server has stopped");
            }

            // End-of-program wait (to preserve separate console windows until user is ready to close them)
            Logger.Info("-- Instance Terminated --" + Environment.NewLine + "Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Start a new instance of the publish-subscribe broker test application in a new console window with the specified arguments
        /// </summary>
        /// <param name="arguments">The applications to pass to the new application instance</param>
        /// <returns>The newly-created Process for the application instance</returns>
        private static Process StartNewAppInstance(string arguments)
        {
            Process newInstance = new Process();
            newInstance.StartInfo.FileName = Process.GetCurrentProcess().ProcessName + ".exe";
            newInstance.StartInfo.Arguments = arguments;
            newInstance.StartInfo.UseShellExecute = true; // Ensures the new instance has its own console window
            newInstance.Start();
            return newInstance;
        }

        /// <summary>
        /// Initialize a new broker server at the specified IP and port to test with
        /// </summary>
        /// <returns>The new BrokerServer instance</returns>
        private static BrokerServer StartBrokerServer(string ipAddress, int port)
        {
            BrokerServer server = new BrokerServer(ipAddress, port);
            try
            {
                // Start the broker server (it will listen for clients in a separate thread)
                server.StartServer();

                Logger.Info("Broker server started");
            }
            catch (Exception e)
            {
                Logger.Error("Broker server failed to start" + Environment.NewLine + e.Message);
            }
            return server;
        }

        /// <summary>
        /// Initialize a new subscriber client for the specified IP and port to test with
        /// </summary>
        /// <param name="topics">An array of topic names to subscribe to</param>
        private static void StartSubscriberClient(string ipAddress, int port, string[] topics)
        {
            try
            {
                // Start the subscriber client
                SubscriberClient client = new SubscriberClient(ipAddress, port);
                client.StartClient();

                Logger.Info("Subscriber client started");

                // Wait a few seconds for publishers to create their topics
                Logger.Info("Waiting for publishers to create topics...");
                Thread.Sleep(1500);

                // Request a list of topics
                Logger.Info("Requesting list of topics...");
                client.AddRequest(new ListTopicsRequest());
                Thread.Sleep(1500);

                // Subscribe to the topics specified in the arguments
                foreach (string topic in topics)
                {
                    Logger.Info("Requesting subscription to topic \"" + topic + "\"...");
                    Guid topicId = client.FindTopicID(topic);
                    client.AddRequest(client.MakeSubscriptionRequest(topicId));
                }

                // Keep process alive while the client is connected
                while (client.IsConnected())
                {
                    Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Subscriber client failed to start" + Environment.NewLine + e.Message);
            }
        }

        /// <summary>
        /// Initialize a new publisher client for the specified IP and port to test with
        /// </summary>
        /// <param name="topic">The name of the publisher's topic (created upon starting the publisher client)</param>
        private static void StartPublisherClient(string ipAddress, int port, string topic)
        {
            try
            {
                // Start the publisher client
                PublisherClient client = new PublisherClient(ipAddress, port);
                client.StartClient();

                Logger.Info("Publisher client started");

                // Create a topic
                Logger.Info("Requesting creation of topic \"" + topic + "\"...");
                client.AddRequest(new CreateTopicRequest(topic));
                Thread.Sleep(1000);

                // Give subscribers time to subscribe
                Logger.Info("Waiting for subscribers...");
                Thread.Sleep(5000);

                // Publish some random-data messages to the topic
                Random rand = new Random();
                for (int i = 0; i < 10; i++)
                {
                    string message = RandomText.Substring(rand.Next(0, RandomText.Length - 21), 20);
                    Logger.Info("Attempting to publish message: " + message);
                    client.AddRequest(client.MakePublishRequest(client.OwnedTopics[0], message));
                    Thread.Sleep(rand.Next(1000, 6000));
                }
            }
            catch (Exception e)
            {
                Logger.Error("Publisher client failed to start" + Environment.NewLine + e.Message);
            }
        }

        // Some random string text courtesy of https://www.random.org/strings/
        private static string RandomText = "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";
    }
}
