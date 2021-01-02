using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    // Todo: This is Cirrus' Dto, response type will change to a domain model
    public class CallCirrusSearchSmartContractTransactionReceiptsWithFilterQuery : IRequest<IEnumerable<ReceiptDto>>
    {
        public CallCirrusSearchSmartContractTransactionReceiptsWithFilterQuery(string contractAddress, ulong from, ulong to, string eventName)
        {
            ContractAddress = contractAddress.HasValue() ? contractAddress : throw new ArgumentNullException(nameof(contractAddress));
            From = from > 0 ? from : throw new ArgumentOutOfRangeException(nameof(from), "From block must be greater than 0.");
            To = to > 0 ? to : throw new ArgumentOutOfRangeException(nameof(to), "To block must be greater than 0.");
            EventName = eventName.HasValue() ? eventName : throw new ArgumentNullException(nameof(eventName));
        }
        
        public string ContractAddress { get; }
        public ulong From { get;  }
        public ulong To { get; }
        public string EventName { get; }
    }
}