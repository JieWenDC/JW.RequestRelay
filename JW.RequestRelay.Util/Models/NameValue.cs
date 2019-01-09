using System;

namespace JW.RequestRelay.Util.Models
{
    /// <summary>
    /// Can be used to store Name/Value (or Key/Value) pairs.
    /// </summary>
    [Serializable]
    public partial class NameValue
    {
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates a new <see cref="NameValue"/>.
        /// </summary>
        public NameValue()
        {

        }

        /// <summary>
        /// Creates a new <see cref="NameValue"/>.
        /// </summary>
        public NameValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
