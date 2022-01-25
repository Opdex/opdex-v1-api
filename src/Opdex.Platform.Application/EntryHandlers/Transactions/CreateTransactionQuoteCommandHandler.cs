using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Domain.Models.Transactions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions;

public class CreateTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateTransactionQuoteCommand>
{
    public CreateTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                IMediator mediator, OpdexConfiguration config)
        : base(quoteAssembler, mediator, config)
    {
    }

    public override async Task<TransactionQuoteDto> Handle(CreateTransactionQuoteCommand request, CancellationToken cancellationToken)
    {
        var parameters = request.Parameters.Select(
            p => new TransactionQuoteRequestParameter(p.Item1, SmartContractMethodParameter.Deserialize(p.Item2)))
            .ToList().AsReadOnly();
        var quote = new TransactionQuoteRequest(request.Sender, request.To, request.Amount, request.Method, request.Callback, parameters);
        return await ExecuteAsync(quote, cancellationToken);
    }
}
