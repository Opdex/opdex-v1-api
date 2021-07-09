using MediatR;
using Opdex.Platform.Common.Enums;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Blocks
{
    public class ProcessLatestBlocksCommand : IRequest<Unit>
    {
        public ProcessLatestBlocksCommand(NetworkType networkType)
        {
            if (networkType != NetworkType.DEVNET && networkType != NetworkType.TESTNET && networkType != NetworkType.MAINNET)
            {
                throw new ArgumentOutOfRangeException(nameof(networkType), "Invalid network type.");
            }

            NetworkType = networkType;
        }

        public NetworkType NetworkType { get; }
    }
}
