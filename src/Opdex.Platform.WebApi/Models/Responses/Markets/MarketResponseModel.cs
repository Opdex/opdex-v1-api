using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.Markets
{
    public class MarketResponseModel
    {
        [NotNull]
        public Address Address { get; set; }

        public Address PendingOwner { get; set; }

        [NotNull]
        public Address Owner { get; set; }

        [NotNull]
        public TokenResponseModel StakingToken { get; set; }

        [NotNull]
        public TokenResponseModel CrsToken { get; set; }

        [NotNull]
        public bool AuthPoolCreators { get; set; }

        [NotNull]
        public bool AuthTraders { get; set; }

        [NotNull]
        public bool AuthProviders { get; set; }

        [NotNull]
        public bool MarketFeeEnabled { get; set; }

        [NotNull]
        public uint TransactionFee { get; set; }

        [NotNull]
        public MarketSummaryResponseModel Summary { get; set; }
    }
}
