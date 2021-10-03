using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults
{
    /// <summary>
    /// Create a vault command to create a new vault.
    /// </summary>
    public class CreateVaultCommand : IRequest<ulong>
    {
        /// <summary>
        /// Constructor to initialize a create vault command.
        /// </summary>
        /// <param name="vault">The address of the vault contract.</param>
        /// <param name="tokenId">The tokenId of the locked token.</param>
        /// <param name="owner">The address of the vault owner.</param>
        /// <param name="blockHeight">The block height the vault was created at.</param>
        public CreateVaultCommand(Address vault, ulong tokenId, Address owner, ulong blockHeight)
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
                throw new ArgumentNullException(nameof(owner), "Owner address must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Vault = vault;
            TokenId = tokenId;
            Owner = owner;
            BlockHeight = blockHeight;
        }

        public Address Vault { get; }
        public ulong TokenId { get; }
        public Address Owner { get; }
        public ulong BlockHeight { get; }
    }
}
