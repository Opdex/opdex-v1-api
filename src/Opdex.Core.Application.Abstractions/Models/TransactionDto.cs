using System.Collections.Generic;
using System.Linq;
using Opdex.Core.Application.Abstractions.Models.TransactionEvents;

namespace Opdex.Core.Application.Abstractions.Models
{
    public class TransactionDto
    {
        public TransactionDto()
        {
            Events = Enumerable.Empty<TransactionEventDto>();
        }
        
        public long Id { get; set; }
        public string Hash { get; set; }
        public ulong BlockHeight { get; set; }
        public int GasUsed { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public IEnumerable<TransactionEventDto> Events { get; set; }
    }
}