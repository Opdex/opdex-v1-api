using System.Collections.Generic;
using System.Linq;
using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Models
{
    public class TransactionDto
    {
        public TransactionDto()
        {
            Logs = Enumerable.Empty<TransactionLogDto>();
        }
        
        public long Id { get; set; }
        public string Hash { get; set; }
        public ulong BlockHeight { get; set; }
        public int GasUsed { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public IEnumerable<TransactionLogDto> Logs { get; set; }
    }
}