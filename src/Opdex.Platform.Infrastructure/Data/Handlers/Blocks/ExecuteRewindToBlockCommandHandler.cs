using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Blocks
{
    public class ExecuteRewindToBlockCommandHandler : IRequestHandler<ExecuteRewindToBlockCommand, bool>
    {
        private const string SqlCommand = "RewindToBlock";

        private readonly IDbContext _context;
        private readonly ILogger _logger;

        public ExecuteRewindToBlockCommandHandler(IDbContext context, ILogger<ExecuteRewindToBlockCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ExecuteRewindToBlockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var command = DatabaseQuery.Create(SqlCommand, CommandType.StoredProcedure, new {request.Block}, cancellationToken);

                await _context.ExecuteCommandAsync(command);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure rewinding to block {request.Block}.");

                return false;
            }
        }
    }
}
