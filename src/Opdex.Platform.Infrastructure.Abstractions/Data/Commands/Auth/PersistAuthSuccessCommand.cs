using MediatR;
using Opdex.Platform.Domain.Models.Auth;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Auth;

public class PersistAuthSuccessCommand : IRequest<bool>
{
    public PersistAuthSuccessCommand(AuthSuccess authSuccess)
    {
        AuthSuccess = authSuccess;
    }

    public AuthSuccess AuthSuccess { get; }
}
