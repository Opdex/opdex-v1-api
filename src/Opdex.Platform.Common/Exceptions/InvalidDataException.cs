using System;

namespace Opdex.Platform.Common.Exceptions
{
    public class InvalidDataException : Exception
    {
        public InvalidDataException(string propertyName, string message) : base(message)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}
