using System;

namespace Opdex.Platform.WebApi.Exceptions;

public class TooManyRequestsException : Exception
{
    public TooManyRequestsException(double limit, string period, string retryAfter)
    {
        Limit = limit;
        Period = period;
        RetryAfter = retryAfter;
    }

    public double Limit { get; }
    public string Period { get; }
    public string RetryAfter { get; }
}