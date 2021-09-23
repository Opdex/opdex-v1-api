using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Domain.Models.Markets
{
    public class MarketContractSummary
    {
        public MarketContractSummary(ulong blockHeight)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            BlockHeight = blockHeight;
        }

        public ulong BlockHeight { get; }
        public Address? Owner { get; private set; }

        public void SetOwner(SmartContractMethodParameter value)
        {
            if (value.Type != SmartContractParameterType.Address)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Owner value must be of Address type.");
            }

            var owner = value.Parse<Address>();

            Owner = owner;
        }
    }
}
