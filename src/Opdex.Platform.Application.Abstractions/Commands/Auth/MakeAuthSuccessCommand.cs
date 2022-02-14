using MediatR;
using Opdex.Platform.Domain.Models.Auth;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Auth;

public class MakeAuthSuccessCommand : IRequest<bool>
{
    public MakeAuthSuccessCommand(AuthSuccess authSuccess)
    {
        AuthSuccess = authSuccess ?? throw new ArgumentNullException(nameof(authSuccess), "Authentication success details must be provided.");
    }

    public AuthSuccess AuthSuccess { get; }
}
