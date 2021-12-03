using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Exceptions;

public class TokenAlreadyIndexedException : Exception
{
    public TokenAlreadyIndexedException(Address token)
    {
        Token = token;
    }

    public Address Token { get; }
}