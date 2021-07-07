using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressAllowance : BlockAudit
    {
        public AddressAllowance(long tokenId, string owner, string spender, string allowance, ulong createdBlock)
            : base(createdBlock)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be set.");
            }

            if (!spender.HasValue())
            {
                throw new ArgumentNullException(nameof(spender), "Spender must be set.");
            }

            if (!allowance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(allowance), "Allowance must only contain numeric digits.");
            }

            TokenId = tokenId;
            Owner = owner;
            Spender = spender;
            Allowance = allowance;
        }

        public AddressAllowance(long id, long tokenId, string owner, string spender, string allowance,
            ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            TokenId = tokenId;
            Owner = owner;
            Spender = spender;
            Allowance = allowance;
        }

        public long Id { get; }
        public long TokenId { get; }

        public string Owner { get; }
        public string Spender { get; }
        public string Allowance { get; private set; }

        public void SetAllowance(string amount, ulong blockHeight)
        {
            if (!amount.IsNumeric()) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must only contain numeric digits.");

            Allowance = amount;
            SetModifiedBlock(blockHeight);
        }
    }
}
