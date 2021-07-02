using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Common.Configurations
{
    public class OpdexConfiguration : IValidatable
    {
        public string ConnectionString { get; set; }
        public NetworkType Network { get; set; }

        public void Validate()
        {
            if (!ConnectionString.HasValue())
            {
                throw new Exception($"{nameof(OpdexConfiguration)}.{nameof(ConnectionString)} must not be null or empty.");
            }

            if (Network != NetworkType.DEVNET &&
                Network != NetworkType.TESTNET &&
                Network != NetworkType.MAINNET)
            {
                throw new Exception($"{nameof(OpdexConfiguration)}.{nameof(Network)} must not be a valid network type.");
            }
        }
    }
}
