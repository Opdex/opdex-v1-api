using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Opdex.Indexer.Application
{
    public class IndexProcessManager
    {
        private readonly ILogger<IndexProcessManager> _logger;
        
        public IndexProcessManager(ILogger<IndexProcessManager> logger)
        {
            _logger = logger;
        }

        public async Task Process()
        {
            // 1. Ping CFN
            // 2. Get LastSyncedBlock
            // 3. Process if latest is newer than LastSyncedBlock
            //     a. Index any new pairs and any new tokens
            //     b. Foreach Pair
            //          i. Index Mint Events
            //              - Index transaction and event if not exists
            //          i. Index Burn Events
            //              - Index transaction and event if not exists
            //          i. Index Swap Events
            //              - Index transaction and event if not exists
            //
            // Log and swallow all exceptions
        }
    }
}