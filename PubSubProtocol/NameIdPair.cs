using System;

namespace PublishSubscribeBroker
{
    /// <summary>
    /// Simple data object containing a string name and associated unique ID, used for identifying publishers, subscribers, and topics
    /// </summary>
    [Serializable]
    public class NameIdPair
    {
        /// <summary>
        /// The readable name of the associated object
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique ID of the associated object
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Construct a new NameIdPair with the specified name and unique ID
        /// </summary>
        /// <param name="name">The name of the associated object</param>
        /// <param name="id">The unique ID of the associated object</param>
        public NameIdPair(string name, Guid id)
        {
            Name = name;
            ID = id;
        }
    }
}
