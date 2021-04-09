using System.Collections.Generic;
using System.Linq;
using Opdex.Platform.WebApi.Models.Responses.TransactionLogs;

namespace Opdex.Platform.WebApi.Models.Responses
{
    public class TransactionResponseModel
    {
        public TransactionResponseModel()
        {
            Logs = Enumerable.Empty<TransactionLogResponseModelBase>();
        }
        
        public string Hash { get; set; }
        public ulong BlockHeight { get; set; }
        public int GasUsed { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public IEnumerable<TransactionLogResponseModelBase> Logs { get; set; }
    }
}