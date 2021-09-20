using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults
{
    public class CreateVaultCommand : IRequest<long>
    {
        public CreateVaultCommand(Address vault, long tokenId, Address owner, ulong blockHeight, bool isUpdate = false)
        {
            if (vault == Address.Empty)
            {
                throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
            }

            if (tokenId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "Token Id must be greater than zero.");
            }

            if (owner == Address.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(owner), "Owner must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Vault = vault;
            TokenId = tokenId;
            Owner = owner;
            BlockHeight = blockHeight;
            IsUpdate = isUpdate;
        }

        public Address Vault { get; }
        public long TokenId { get; }
        public Address Owner { get; }
        public ulong BlockHeight { get; }
        public bool IsUpdate { get; }
    }
}
