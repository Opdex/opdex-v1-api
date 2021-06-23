using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands
{
    /// <summary>
    /// First block of the day, create all new daily snapshots for each token, pool, and market
    /// Note: This is not necessarily built to scale... May need to be revisited in the future.
    /// </summary>
    public class ProcessDailySnapshotRefreshCommand : IRequest<Unit>
    {
        /// <summary>
        /// Constructor creating new process daily snapshot refresh command.
        /// </summary>
        /// <param name="blockTime">Current block time</param>
        /// <param name="crsUsd">Current CRS USd price</param>
        public ProcessDailySnapshotRefreshCommand(DateTime blockTime, decimal crsUsd)
        {
            if (blockTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(blockTime), $"{nameof(blockTime)} must be valid.");
            }

            if (crsUsd < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(crsUsd), $"{nameof(crsUsd)} must be greater than 0.");
            }

            Blocktime = blockTime;
            CrsUsd = crsUsd;
        }

        public DateTime Blocktime { get; }
        public decimal CrsUsd { get; }
    }
}