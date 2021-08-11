using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Application.Assemblers
{
    public class TransactionDtoAssembler : IModelAssembler<Transaction, TransactionDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IModelAssembler<IEnumerable<TransactionLog>, IReadOnlyCollection<TransactionEventDto>> _eventsAssembler;

        public TransactionDtoAssembler(IMediator mediator,
                                       IMapper mapper,
                                       IModelAssembler<IEnumerable<TransactionLog>, IReadOnlyCollection<TransactionEventDto>> eventsAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventsAssembler = eventsAssembler ?? throw new ArgumentNullException(nameof(eventsAssembler));
        }

        public async Task<TransactionDto> Assemble(Transaction tx)
        {
            var transactionDto = _mapper.Map<TransactionDto>(tx);

            var txLogs = await _mediator.Send(new RetrieveTransactionLogsByTransactionIdQuery(tx.Id));

            var logList = txLogs?.ToList() ?? new List<TransactionLog>();
            if (logList.Any())
            {
                transactionDto.Events = await _eventsAssembler.Assemble(logList);
            }

            var block = await _mediator.Send(new RetrieveBlockByHeightQuery(tx.BlockHeight));

            transactionDto.BlockDto = _mapper.Map<BlockDto>(block);

            return transactionDto;
        }
    }
}
