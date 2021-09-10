using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    // Todo: This is Cirrus' Dto, response type will change to a domain model
    public class CallCirrusSearchContractTransactionsQuery : IRequest<List<Transaction>>
    {
        public CallCirrusSearchContractTransactionsQuery(Address contractAddress, string logName, ulong from, ulong? to = null)
        {
            ContractAddress = contractAddress != Address.Empty ? contractAddress : throw new ArgumentNullException(nameof(contractAddress));
            From = from > 0 ? from : throw new ArgumentOutOfRangeException(nameof(from), "From block must be greater than 0.");
            LogName = logName.HasValue() ? logName : throw new ArgumentNullException(nameof(logName));
            To = to;
        }

        public Address ContractAddress { get; }
        public ulong From { get; }
        public ulong? To { get; }
        public string LogName { get; }
    }
}
