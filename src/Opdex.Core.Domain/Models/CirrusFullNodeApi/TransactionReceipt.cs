using System;
using System.Collections.Generic;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models.CirrusFullNodeApi
{
    public class TransactionReceipt
    {
        public TransactionReceipt(string txHash, string blockHash, string from, string to, 
            ulong gasUsed, string newContractAddress, bool success, string returnValue)
        {
            TxHash = txHash.HasValue() ? txHash : throw new ArgumentNullException(nameof(txHash));
            BlockHash = blockHash.HasValue() ? blockHash : throw new ArgumentNullException(nameof(blockHash));
            From = from.HasValue() ? from : throw new ArgumentNullException(nameof(from));
            To = to.HasValue() ? to : throw new ArgumentNullException(nameof(to));

            GasUsed = gasUsed;
            NewContractAddress = newContractAddress;
            Success = success;
            ReturnValue = returnValue;
        }
        
        public string TxHash { get; }
        public string BlockHash { get; }
        public ulong GasUsed { get; }
        public string From { get; }
        public string To { get; }
        public string NewContractAddress { get; }
        public bool Success { get; }
        public string ReturnValue { get; }
        
        // Todo: Set in constructor or in different method
        public List<ITransactionReceiptEvent> Events { get; private set; }

        public bool CreatedNewContract => NewContractAddress.HasValue();
    }
}