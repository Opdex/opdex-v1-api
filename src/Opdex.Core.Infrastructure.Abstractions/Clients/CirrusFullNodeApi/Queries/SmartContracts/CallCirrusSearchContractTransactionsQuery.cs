using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    // Todo: This is Cirrus' Dto, response type will change to a domain model
    public class CallCirrusSearchContractTransactionsQuery : IRequest<List<Transaction>>
    {
        public CallCirrusSearchContractTransactionsQuery(string contractAddress, string eventName, ulong from, ulong? to = null)
        {
            ContractAddress = contractAddress.HasValue() ? contractAddress : throw new ArgumentNullException(nameof(contractAddress));
            From = from > 0 ? from : throw new ArgumentOutOfRangeException(nameof(from), "From block must be greater than 0.");
            EventName = eventName.HasValue() ? eventName : throw new ArgumentNullException(nameof(eventName));
            To = to;
        }
        
        public string ContractAddress { get; }
        public ulong From { get;  }
        public ulong? To { get; }
        public string EventName { get; }
    }
}