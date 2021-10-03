using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressAllowance : BlockAudit
    {
        public AddressAllowance(ulong tokenId, Address owner, Address spender, UInt256 allowance, ulong createdBlock)
            : base(createdBlock)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
            }

            if (owner == Address.Empty)
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be set.");
            }

            if (spender == Address.Empty)
            {
                throw new ArgumentNullException(nameof(spender), "Spender must be set.");
            }

            TokenId = tokenId;
            Owner = owner;
            Spender = spender;
            Allowance = allowance;
        }

        public AddressAllowance(ulong id, ulong tokenId, Address owner, Address spender, UInt256 allowance,
            ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            TokenId = tokenId;
            Owner = owner;
            Spender = spender;
            Allowance = allowance;
        }

        public ulong Id { get; }
        public ulong TokenId { get; }

        public Address Owner { get; }
        public Address Spender { get; }
        public UInt256 Allowance { get; private set; }

        public void SetAllowance(UInt256 amount, ulong blockHeight)
        {
            Allowance = amount;
            SetModifiedBlock(blockHeight);
        }
    }
}
