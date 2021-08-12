using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Application.Abstractions.Models.Transactions
{
    public class TransactionDto
    {
        public TransactionDto()
        {
            Events = Enumerable.Empty<TransactionEventDto>();
        }

        public bool Success { get; set; }
        public string Hash { get; set; }
        public string NewContractAddress { get; set; }
        public BlockDto BlockDto { get; set; }
        public int GasUsed { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public IEnumerable<TransactionEventDto> Events { get; set; }
    }
}
