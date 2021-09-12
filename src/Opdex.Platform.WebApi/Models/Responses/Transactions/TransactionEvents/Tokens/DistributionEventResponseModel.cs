using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens
{
    public class DistributionEventResponseModel : TransactionEventResponseModel
    {
        public FixedDecimal VaultAmount { get; set; }
        public FixedDecimal GovernanceAmount { get; set; }
        public uint PeriodIndex { get; set; }
        public FixedDecimal TotalSupply { get; set; }
        public ulong NextDistributionBlock { get; set; }
    }
}
