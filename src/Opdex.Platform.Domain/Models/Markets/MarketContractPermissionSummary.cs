using System;

namespace Opdex.Platform.Domain.Models.Markets;

public class MarketContractPermissionSummary
{
    public MarketContractPermissionSummary(ulong blockHeight)
    {
        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
    public bool? Authorization { get; private set; }

    public void SetAuthorization(bool value)
    {
        Authorization = value;
    }
}