using Opdex.Platform.WebApi.Models.Responses.Blocks;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions
{
    public class TransactionResponseModel
    {
        public TransactionResponseModel()
        {
            Events = Enumerable.Empty<TransactionEventResponseModel>();
        }

        public bool Success { get; set; }
        public string Hash { get; set; }
        public string NewContractAddress { get; set; }
        public BlockResponseModel Block { get; set; }
        public int GasUsed { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public IEnumerable<TransactionEventResponseModel> Events { get; set; }
    }
}
