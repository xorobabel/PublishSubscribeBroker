using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PublishSubscribeBroker.Networking
{
    // Message object for a simple message-based communication protocol
    class Message
    {
        byte[] Data { get; set; } // The raw data representing the message's contents

        // Create a Message from the provided data
        public Message(byte[] data)
        {
            this.Data = data;
        }

        // Encode the Message into a communicable format
        // -- The message length will be in the first 4 bytes of the resulting byte array, followed by the message data
        public byte[] Encode()
        {
            byte[] encodedMessage = new byte[Data.Length + 4];
            byte[] lengthBytes = BitConverter.GetBytes(Data.Length); // Convert the message length (int) to bytes

            // Put message length in first 4 bytes, then append the message data
            lengthBytes.CopyTo(encodedMessage, 0);
            Data.CopyTo(encodedMessage, 4);

            return encodedMessage;
        }

        // Decode a Message object from encoded/communicable message data
        public static Message Decode(byte[] message)
        {
            int length = BitConverter.ToInt32(message, 0); // Convert the encoded message length to an integer
            byte[] messageData = new byte[length];

            // Copy remaining bytes to the data array
            for (int i = 0; i < length; i++)
                messageData[i] = message[i + 4];

            return new Message(messageData);
        }

    }
}
