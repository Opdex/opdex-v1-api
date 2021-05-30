using System;

namespace Opdex.Platform.Common.Exceptions
{
    public class IndexingAlreadyRunningException : Exception
    {
        public IndexingAlreadyRunningException() : base("Indexing is already running, try again later.")
        {
        }
    }
}