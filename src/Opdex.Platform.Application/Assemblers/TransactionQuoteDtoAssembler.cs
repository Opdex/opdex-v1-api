using AutoMapper;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class TransactionQuoteDtoAssembler : IModelAssembler<TransactionQuote, TransactionQuoteDto>
{
    private readonly IMapper _mapper;
    private readonly IModelAssembler<IEnumerable<TransactionLog>, IReadOnlyCollection<TransactionEventDto>> _eventsAssembler;

    public TransactionQuoteDtoAssembler(IMapper mapper,
                                        IModelAssembler<IEnumerable<TransactionLog>, IReadOnlyCollection<TransactionEventDto>> eventsAssembler)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _eventsAssembler = eventsAssembler ?? throw new ArgumentNullException(nameof(eventsAssembler));
    }

    public async Task<TransactionQuoteDto> Assemble(TransactionQuote quote)
    {
        var transactionQuote = _mapper.Map<TransactionQuoteDto>(quote);

        // Todo: This is shard so we can show events in rich context, however, we may want to quote users exclusively using Cirrus FN
        transactionQuote.Events = await _eventsAssembler.Assemble(quote.Logs);

        return transactionQuote;
    }
}