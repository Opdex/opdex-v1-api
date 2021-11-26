using System;

namespace Opdex.Platform.Common.Exceptions
{
    /// <summary>
    /// An exception that is used when data makes a request invalid.
    /// </summary>
    public class InvalidDataException : Exception
    {
        /// <summary>
        /// Creates an invalid data exception which specifies a specific request property is invalid.
        /// </summary>
        /// <param name="propertyName">The name of the property in the request.</param>
        /// <param name="message">Error message.</param>
        public InvalidDataException(string propertyName, string message) : base(message)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}
