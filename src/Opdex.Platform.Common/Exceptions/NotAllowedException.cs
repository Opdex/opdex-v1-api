using System;

namespace Opdex.Platform.Common.Exceptions;

public class NotAllowedException : Exception
{
    public NotAllowedException(string message) : base(message)
    {
    }
}
