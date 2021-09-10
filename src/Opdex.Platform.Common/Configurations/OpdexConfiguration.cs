using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Common.Configurations
{
    public class OpdexConfiguration : IValidatable
    {
        private string _walletTransactionCallback;

        public string ApiUrl { get; set; }

        public string WalletTransactionCallback
        {
            get => $"{this.ApiUrl}{this._walletTransactionCallback}";
            set => _walletTransactionCallback = value;
        }

        public NetworkType Network { get; set; }

        public string InstanceId { get; } = Guid.NewGuid().ToString();

        public void Validate()
        {
            if (!ApiUrl.HasValue())
            {
                throw new Exception($"{nameof(OpdexConfiguration)}.{nameof(ApiUrl)} must not be null or empty.");
            }

            if (!WalletTransactionCallback.HasValue())
            {
                throw new Exception($"{nameof(OpdexConfiguration)}.{nameof(WalletTransactionCallback)} must not be null or empty.");
            }

            if (!Network.IsValid())
            {
                throw new Exception($"{nameof(OpdexConfiguration)}.{nameof(Environment)} must be a valid network type.");
            }
        }
    }
}
