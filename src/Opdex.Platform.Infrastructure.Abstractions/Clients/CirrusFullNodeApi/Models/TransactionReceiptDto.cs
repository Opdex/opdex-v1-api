using Opdex.Platform.Common.Models;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class TransactionReceiptDto
    {
        public Sha256 TransactionHash { get; set; }
        public Sha256 BlockHash { get; set; }
        public string PostState { get; set; }
        public int GasUsed { get; set; }
        public Address From { get; set; }
        public Address To { get; set; }
        public Address NewContractAddress { get; set; }
        public bool Success { get; set; }
        public string ReturnValue { get; set; }
        public string Bloom { get; set; }
        public string Error { get; set; }

        public ulong BlockHeight { get; private set; }

        public IList<TransactionLogDto> Logs { get; set; }

        public void SetBlockHeight(ulong height)
        {
            if (height > 1)
            {
                BlockHeight = height;
            }
        }
    }
}
