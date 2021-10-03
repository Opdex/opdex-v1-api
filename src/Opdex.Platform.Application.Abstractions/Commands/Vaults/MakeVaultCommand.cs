using System;
using MediatR;
using Opdex.Platform.Domain.Models.Vaults;

namespace Opdex.Platform.Application.Abstractions.Commands.Vaults
{
    /// <summary>
    /// Create a make vault command to persist a vault domain model. Include refresh parameters to refresh the
    /// included vault properties prior to persistence.
    /// </summary>
    public class MakeVaultCommand : IRequest<ulong>
    {
        /// <summary>
        /// Constructor to make a vault command.
        /// </summary>
        /// <param name="vault">The vault domain model to update and/or persist.</param>
        /// <param name="blockHeight">The block height used to refresh select properties when applicable based on associated refresh parameters.</param>
        /// <param name="refreshPendingOwner">Flag to refresh the pending owner value from contract state, default is false.</param>
        /// <param name="refreshOwner">Flag to refresh the owner value from contract state, default is false.</param>
        /// <param name="refreshSupply">Flag to refresh the supply value from contract state, default is false.</param>
        /// <param name="refreshGenesis">Flag to refresh the genesis block value from contract state, default is false.</param>
        public MakeVaultCommand(Vault vault, ulong blockHeight, bool refreshPendingOwner = false, bool refreshOwner = false, bool refreshSupply = false, bool refreshGenesis = false)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Vault = vault ?? throw new ArgumentNullException(nameof(vault), "Vault must be provided.");
            BlockHeight = blockHeight;
            RefreshPendingOwner = refreshPendingOwner;
            RefreshOwner = refreshOwner;
            RefreshSupply = refreshSupply;
            RefreshGenesis = refreshGenesis;
        }

        public Vault Vault { get; }
        public ulong BlockHeight { get; }
        public bool RefreshPendingOwner { get; }
        public bool RefreshOwner { get; }
        public bool RefreshSupply { get; }
        public bool RefreshGenesis { get; }
        public bool Refresh => RefreshPendingOwner || RefreshOwner || RefreshSupply || RefreshGenesis;
    }
}
