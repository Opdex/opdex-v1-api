using System;
using System.Collections.Generic;
using System.Linq;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Indexer.Domain.Models
{
    public class TransactionReceipt
    {
        public TransactionReceipt(TransactionReceiptDto transactionReceiptDto)
        {
            if (transactionReceiptDto == null)
            {
                throw new ArgumentNullException(nameof(transactionReceiptDto));
            }

            if (!transactionReceiptDto.TransactionHash.HasValue())
            {
                throw new ArgumentNullException(nameof(transactionReceiptDto.TransactionHash));
            }
            
            if (!transactionReceiptDto.BlockHash.HasValue())
            {
                throw new ArgumentNullException(nameof(transactionReceiptDto.BlockHash));
            }
            
            if (transactionReceiptDto.GasUsed == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(transactionReceiptDto.GasUsed));
            }
            
            if (!transactionReceiptDto.From.HasValue())
            {
                throw new ArgumentNullException(nameof(transactionReceiptDto.From));
            }
            
            if (!transactionReceiptDto.To.HasValue())
            {
                throw new ArgumentNullException(nameof(transactionReceiptDto.To));
            }

            if (!transactionReceiptDto.Success)
            {
                throw new ArgumentException($"Transaction must be a {nameof(transactionReceiptDto.Success)}", nameof(transactionReceiptDto.Success));
            }
            
            if (!transactionReceiptDto.Logs.Any())
            {
                throw new ArgumentException("Transaction Receipt must include logs", nameof(transactionReceiptDto.Logs));
            }

            Hash = transactionReceiptDto.TransactionHash;
            BlockHash = transactionReceiptDto.BlockHash;
            GasUsed = transactionReceiptDto.GasUsed;
            From = transactionReceiptDto.From;
            To = transactionReceiptDto.To;
            Success = transactionReceiptDto.Success;
            Events = transactionReceiptDto.Logs.Select(log => new TransactionLog(log)).ToList();
        }
        
        public string Hash { get; private set; }
        public string BlockHash { get; private set; }
        public int GasUsed { get; private set; }
        public string From { get; private set; }
        public string To { get; private set; }
        public bool Success { get; private set; }
        public IReadOnlyCollection<TransactionLog> Events { get; private set; }
    }
}