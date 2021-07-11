using System;

namespace Opdex.Platform.Common.Exceptions
{
    public class IndexingAlreadyRunningException : Exception
    {
        private const string MessageBase = "Indexing is already running, try again later.";

        public IndexingAlreadyRunningException() : this(MessageBase)
        {
        }

        public IndexingAlreadyRunningException(string message) : base(message)
        {
        }
    }
}
