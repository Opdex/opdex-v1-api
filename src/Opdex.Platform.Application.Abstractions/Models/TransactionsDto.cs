using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models
{
    public class TransactionsDto
    {
        public IEnumerable<TransactionDto> TransactionDtos { get; set; }
        public CursorDto Cursor { get; set; }
    }
}
