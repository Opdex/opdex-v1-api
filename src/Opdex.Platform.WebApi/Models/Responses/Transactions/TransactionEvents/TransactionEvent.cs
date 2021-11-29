using Newtonsoft.Json;
using NJsonSchema.Converters;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Deployers;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningGovernances;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Markets;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vault;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents
{
    /// <summary>
    /// Event that occurred in a transaction.
    /// </summary>
    [JsonConverter(typeof(JsonInheritanceConverter), "eventType")]
    [KnownType(typeof(CreateMarketEvent))]
    [KnownType(typeof(NominationEvent))]
    [KnownType(typeof(RewardMiningPoolEvent))]
    [KnownType(typeof(CollectStakingRewardsEvent))]
    [KnownType(typeof(AddLiquidityEvent))]
    [KnownType(typeof(RemoveLiquidityEvent))]
    [KnownType(typeof(StartStakingEvent))]
    [KnownType(typeof(StopStakingEvent))]
    [KnownType(typeof(SwapEvent))]
    [KnownType(typeof(ChangeMarketPermissionEvent))]
    [KnownType(typeof(CreateLiquidityPoolEvent))]
    [KnownType(typeof(CollectMiningRewardsEvent))]
    [KnownType(typeof(EnableMiningEvent))]
    [KnownType(typeof(StartMiningEvent))]
    [KnownType(typeof(StopMiningEvent))]
    [KnownType(typeof(ClaimPendingDeployerOwnershipEvent))]
    [KnownType(typeof(SetPendingDeployerOwnershipEvent))]
    [KnownType(typeof(ClaimPendingMarketOwnershipEvent))]
    [KnownType(typeof(SetPendingMarketOwnershipEvent))]
    [KnownType(typeof(ClaimPendingVaultOwnershipEvent))]
    [KnownType(typeof(SetPendingVaultOwnershipEvent))]
    [KnownType(typeof(ApprovalEvent))]
    [KnownType(typeof(DistributionEvent))]
    [KnownType(typeof(TransferEvent))]
    [KnownType(typeof(CreateVaultCertificateEvent))]
    [KnownType(typeof(RedeemVaultCertificateEvent))]
    [KnownType(typeof(RevokeVaultCertificateEvent))]
    public abstract class TransactionEvent
    {
        /// <summary>
        /// Event type identifier.
        /// </summary>
        /// <example>SwapEvent</example>
        public TransactionEventType EventType { get; set; }

        /// <summary>
        /// Address of the contract that logged the event.
        /// </summary>
        /// <example>tLrMcU1csbN7RxGjBMEnJeLoae3PxmQ9cr</example>
        [NotNull]
        public Address Contract { get; set; }

        /// <summary>
        /// Position in the sequence the event occurred.
        /// </summary>
        /// <example>1</example>
        public int SortOrder { get; set; }
    }
}
