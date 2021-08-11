using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Common.Configurations
{
    public class OpdexConfiguration : IValidatable
    {
        public string WalletTransactionCallback { get; set; } = "https://dev-api.opdex.com/transactions";
        public string ConnectionString { get; set; }
        public NetworkType Network { get; set; }
        public string InstanceId { get; } = Guid.NewGuid().ToString();

        public void Validate()
        {
            if (!WalletTransactionCallback.HasValue())
            {
                throw new Exception($"{nameof(OpdexConfiguration)}.{nameof(WalletTransactionCallback)} must not be null or empty.");
            }

            if (!ConnectionString.HasValue())
            {
                throw new Exception($"{nameof(OpdexConfiguration)}.{nameof(ConnectionString)} must not be null or empty.");
            }

            if (!Network.IsValid())
            {
                throw new Exception($"{nameof(OpdexConfiguration)}.{nameof(Network)} must be a valid network type.");
            }
        }
    }
}
