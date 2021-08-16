using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    /// <summary>
    /// Mining pool details response
    /// </summary>
    public class MiningPoolResponseModel
    {
        [NotNull]
        public Address Address { get; set; }

        [NotNull]
        public Address LiquidityPool { get; set; }

        [NotNull]
        public ulong MiningPeriodEndBlock { get; set; }

        [NotNull]
        public string RewardPerBlock { get; set; }

        [NotNull]
        public string RewardPerLpt { get; set; }

        [NotNull]
        public string TokensMining { get; set; }

        [NotNull]
        public bool IsActive { get; set; }
    }
}
