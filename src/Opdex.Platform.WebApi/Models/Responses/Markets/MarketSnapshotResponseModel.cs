using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;
using Opdex.Platform.WebApi.Models.Responses.OHLC;
using System;

namespace Opdex.Platform.WebApi.Models.Responses.Markets;

public class MarketSnapshotResponseModel
{
    public OhlcDecimalResponseModel LiquidityUsd { get; set; }
    public decimal VolumeUsd { get; set; }
    public StakingSnapshotResponseModel Staking { get; set; }
    public RewardsSnapshotResponseModel Rewards { get; set; }
    public DateTime Timestamp { get; set; }
}