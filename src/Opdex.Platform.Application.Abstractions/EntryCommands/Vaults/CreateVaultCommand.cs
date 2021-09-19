using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Vaults
{
    public class CreateVaultCommand : IRequest<long>
    {
        public CreateVaultCommand(Address vault, ulong blockHeight, bool isUpdate = false)
        {
            if (vault == Address.Empty)
            {
                throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Vault = vault;
            BlockHeight = blockHeight;
            IsUpdate = isUpdate;
        }

        public Address Vault { get; }
        public ulong BlockHeight { get; }
        public bool IsUpdate { get; }
    }
}
