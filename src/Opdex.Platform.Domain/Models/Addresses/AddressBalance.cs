using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Domain.Models.Addresses
{
    public class AddressBalance : BlockAudit
    {
        public AddressBalance(long tokenId, string owner, string balance, ulong createdBlock) : base(createdBlock)
        {
            if (tokenId < 1)
            {
                throw new ArgumentException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0.");
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be set.");
            }

            if (!balance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(balance), "Balance must only contain numeric digits.");
            }

            TokenId = tokenId;
            Owner = owner;
            Balance = balance;
        }

        public AddressBalance(long id, long tokenId, string owner, string balance, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            TokenId = tokenId;
            Owner = owner;
            Balance = balance;
        }

        public long Id { get; }
        public long TokenId { get; }
        public string Owner { get; }
        public string Balance { get; private set; }

        public void SetBalance(string balance, ulong blockHeight)
        {
            if (!balance.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(balance), "Balance must only contain numeric digits.");
            }

            Balance = balance;
            SetModifiedBlock(blockHeight);
        }
    }
}