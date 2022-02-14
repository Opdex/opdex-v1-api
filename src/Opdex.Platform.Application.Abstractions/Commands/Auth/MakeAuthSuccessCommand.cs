using MediatR;
using Opdex.Platform.Domain.Models.Auth;

namespace Opdex.Platform.Application.Abstractions.Commands.Auth;

public class MakeAuthSuccessCommand : IRequest<bool>
{
    public MakeAuthSuccessCommand(AuthSuccess authSuccess)
    {
        AuthSuccess = authSuccess;
    }

    public AuthSuccess AuthSuccess { get; }
}
