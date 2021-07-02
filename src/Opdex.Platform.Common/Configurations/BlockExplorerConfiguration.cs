using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Common.Configurations
{
    public class BlockExplorerConfiguration : IValidatable
    {
        public string TransactionEndpoint { get; set; }

        public void Validate()
        {
            if (!TransactionEndpoint.HasValue())
            {
                throw new Exception($"{nameof(BlockExplorerConfiguration)}.{nameof(TransactionEndpoint)} must not be null or empty.");
            }
        }
    }
}
