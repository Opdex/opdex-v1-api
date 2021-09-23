using Opdex.Platform.Common.Models;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class TransactionReceiptDto
    {
        private IList<TransactionLogDto> _logs = new List<TransactionLogDto>();
        public string TransactionHash { get; set; }
        public string BlockHash { get; set; }
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

        public IList<TransactionLogDto> Logs
        {
            get
            {
                return _logs;
            }
            set
            {
                for (int i = 0; i < value?.Count; i++) value[i].SortOrder = i;
                _logs = value;
            }
        }

        public void SetBlockHeight(ulong height)
        {
            if (height > 1)
            {
                BlockHeight = height;
            }
        }
    }
}
