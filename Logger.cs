using System;

namespace PublishSubscribeBroker
{
    // Simple logger class to output tagged messages to the console
    class Logger
    {
        public static void Info(string message)
        {
            Console.WriteLine("[INFO] " + message);
        }

        public static void Warn(string message)
        {
            Console.WriteLine("[WARN] " + message);
        }

        public static void Error(string message)
        {
            Console.WriteLine("[ERROR] " + message);
        }
    }
}
