using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace PublishSubscribeBroker.Networking
{
    // This class is looking good

    /// <summary>
    /// Base class for objects that use the network to communicate, providing mechanisms for sending and receiving serialized messages over TCP
    /// </summary>
    class Communicator
    {
        /// <summary>
        /// Serialize the provided serializable object/message to a byte array using a BinaryFormatter
        /// </summary>
        /// <typeparam name="T">The object type to serialize from</typeparam>
        /// <param name="obj">The object/message to serialize</param>
        /// <returns>An array of bytes containing the serialized object/message</returns>
        private byte[] SerializeToBytes<T>(T obj)
        {
            byte[] bytes;
            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, obj);
                bytes = stream.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Deserialize an object/message that has been serialized by a BinaryFormatter into the specified type
        /// </summary>
        /// <typeparam name="T">The object type to deserialize to</typeparam>
        /// <param name="bytes">The byte array containing the object/message data to deserialize</param>
        /// <returns>An object of type T deserialized from the provided byte array</returns>
        protected T DeserializeFromBytes<T>(byte[] bytes)
        {
            T obj;
            using (MemoryStream objStream = new MemoryStream(bytes))
            {
                // Must use typecasting because the "as" keyword does not like generics
                obj = (T) new BinaryFormatter().Deserialize(objStream);
            }
            return obj;
        }

        /// <summary>
        /// Receive a specific number of bytes from the provided network stream
        /// </summary>
        /// <param name="count">The exact number of bytes to receive</param>
        /// <param name="stream">The stream used for network communication</param>
        /// <returns>A byte array containing the requested number of bytes received from the client/server</returns>
        protected byte[] ReceiveBytes(int count, NetworkStream stream)
        {
            int bytesRead = 0;
            byte[] bytes = new byte[count];
            while (bytesRead < bytes.Length)
            {
                // Keep reading from the last point until the byte array is filled
                bytesRead += stream.Read(bytes, bytesRead, bytes.Length - bytesRead);
            }
            return bytes;
        }

        /// <summary>
        /// Receive a complete message object (of the specified type) from the provided network stream
        /// </summary>
        /// <typeparam name="T">The type of message to receive</typeparam>
        /// <param name="stream">The stream used for network communication</param>
        /// <returns>A complete deserialized object of type T received from the client/server</returns>
        protected T ReceiveMessage<T>(NetworkStream stream)
        {
            // Receive length-of-message information (first 4 bytes)
            int length = BitConverter.ToInt32(ReceiveBytes(4, stream), 0);

            // Receive the main message contents
            byte[] messageBytes = ReceiveBytes(length, stream);

            // Deserialize the received message object
            T message = DeserializeFromBytes<T>(messageBytes);

            return message;
        }

        /// <summary>
        /// Format the serialized object/message for sending by prepending the message length in the first 4 bytes
        /// </summary>
        /// <param name="message">The serialized object/message data to prepend the length to</param>
        /// <returns>A byte array containing the message length (4 bytes) followed by the serialized message</returns>
        protected byte[] PrependLength(byte[] message)
        {
            byte[] formatted = new byte[message.Length + 4];
            byte[] lengthBytes = BitConverter.GetBytes(message.Length);

            lengthBytes.CopyTo(formatted, 0);
            message.CopyTo(formatted, 4);

            return formatted;
        }

        /// <summary>
        /// Send a complete message object (of the specified type) to the provided network stream
        /// </summary>
        /// <typeparam name="T">The type of message to send</typeparam>
        /// <param name="stream">The stream used for network communication</param>
        protected void SendMessage<T>(T message, NetworkStream stream)
        {
            // Serialize the message object into binary
            byte[] serializedMessage = SerializeToBytes(message);

            // Format the message by prepending the message length
            byte[] sendableMessage = PrependLength(serializedMessage);

            // Send the message over the network stream
            stream.Write(sendableMessage, 0, sendableMessage.Length);
        }
    }
}
