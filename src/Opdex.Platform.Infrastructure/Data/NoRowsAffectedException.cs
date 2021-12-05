using System;

namespace Opdex.Platform.Infrastructure.Data;

public class NoRowsAffectedException : Exception
{
    public NoRowsAffectedException(string message) : base(message)
    {
    }
}
