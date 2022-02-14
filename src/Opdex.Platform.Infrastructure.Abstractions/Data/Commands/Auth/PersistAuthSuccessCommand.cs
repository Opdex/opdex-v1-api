using MediatR;
using Opdex.Platform.Domain.Models.Auth;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Auth;

public class PersistAuthSuccessCommand : IRequest<bool>
{
    public PersistAuthSuccessCommand(AuthSuccess authSuccess)
    {
        AuthSuccess = authSuccess ?? throw new ArgumentNullException(nameof(authSuccess), "Authentication success details must be provided.");
    }

    public AuthSuccess AuthSuccess { get; }
}
