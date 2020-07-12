using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PublishSubscribeBroker.Networking
{
    /// <summary>
    /// Sendable message object for a simple message-based communication protocol,<br/>
    /// where the content is pre-serialized and can be formatted for sending over a network
    /// <br/><br/>
    /// Serves as a TCP wrapper for sending a serialized object with a length-based data boundary
    /// </summary>
    class SendableMessage<T>
    {
        /// <summary>
        /// The main content of the message in serialized binary format
        /// </summary>
        byte[] Content { get; set; }

        /// <summary>
        /// Create a SendableMessage from the provided content, serializing it immediately
        /// </summary>
        /// <param name="content">The content of the sendable message</param>
        public SendableMessage(T content)
        {
            this.Content = SerializeContent(content);
        }

        /// <summary>
        /// Serialize the provided content object using a BinaryFormatter
        /// </summary>
        /// <param name="content">The content object to serialize</param>
        /// <returns>An array of bytes containing the content object serialized in binary</returns>
        private byte[] SerializeContent(T content)
        {
            byte[] serialized;
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(stream, content);
                serialized = stream.ToArray();
            }
            return serialized;
        }

        /// <summary>
        /// Format the sendable message into a byte array with the content length in the first 4 bytes
        /// </summary>
        /// <returns>A byte array containing the message length (4 bytes) followed by the binary-serialized content</returns>
        public byte[] Format()
        {
            byte[] formatted = new byte[Content.Length + 4];
            byte[] lengthBytes = BitConverter.GetBytes(Content.Length);

            lengthBytes.CopyTo(formatted, 0);
            Content.CopyTo(formatted, 4);

            return formatted;
        }

    }
}
