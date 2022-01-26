using Opdex.Platform.Application.Abstractions.Models.Blocks;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Application.Abstractions.Models.Transactions;

public class TransactionDto : ITransactionDto
{
    public TransactionDto()
    {
        Events = Enumerable.Empty<TransactionEventDto>();
    }

    public bool Success { get; set; }
    public string Error { get; set; }
    public Sha256 Hash { get; set; }
    public Address NewContractAddress { get; set; }
    public BlockDto BlockDto { get; set; }
    public int GasUsed { get; set; }
    public Address From { get; set; }
    public Address To { get; set; }
    public IEnumerable<TransactionEventDto> Events { get; set; }
}
