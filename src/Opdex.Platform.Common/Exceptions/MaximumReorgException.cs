using System;

namespace Opdex.Platform.Common.Exceptions
{
    public class MaximumReorgException : Exception
    {
        public MaximumReorgException() : base("Maximum reorg limit reached")
        {
        }
    }
}
