using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Domain.Models.Vaults
{
    public class VaultContractSummary
    {
        public VaultContractSummary(ulong blockHeight)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            BlockHeight = blockHeight;
        }

        public ulong BlockHeight { get; }
        public Address? LockedToken { get; private set; }
        public ulong? Genesis { get; private set; }
        public Address? Owner { get; private set; }
        public UInt256? UnassignedSupply { get; private set; }

        public void SetLockedToken(SmartContractMethodParameter value)
        {
            var token = value.Parse<Address>();

            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token), "Token address must be provided.");
            }

            LockedToken = token;
        }

        public void SetGenesis(SmartContractMethodParameter value)
        {
            var genesis = value.Parse<ulong>();

            // Zero is valid, nothing to check
            Genesis = genesis;
        }

        public void SetOwner(SmartContractMethodParameter value)
        {
            var owner = value.Parse<Address>();

            if (owner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(owner), "Owner address must be provided.");
            }

            Owner = owner;
        }

        public void SetUnassignedSupply(SmartContractMethodParameter value)
        {
            var supply = value.Parse<UInt256>();

            // Zero is valid, nothing to check
            UnassignedSupply = supply;
        }
    }
}
