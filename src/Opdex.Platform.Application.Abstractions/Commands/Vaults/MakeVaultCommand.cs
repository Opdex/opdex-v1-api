using System;
using MediatR;
using Opdex.Platform.Domain.Models.Vaults;

namespace Opdex.Platform.Application.Abstractions.Commands.Vaults
{
    public class MakeVaultCommand : IRequest<long>
    {
        public MakeVaultCommand(Vault vault, ulong blockHeight, bool rewind = false)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Vault = vault ?? throw new ArgumentNullException(nameof(vault), "Vault must be provided.");
            BlockHeight = blockHeight;
            Rewind = rewind;
        }

        public Vault Vault { get; }
        public ulong BlockHeight { get; }
        public bool Rewind { get; }
    }
}
