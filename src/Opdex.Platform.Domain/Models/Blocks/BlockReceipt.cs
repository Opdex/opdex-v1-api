using System;
using System.Collections.Generic;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.Blocks
{
    public class BlockReceipt
    {
        public BlockReceipt(Sha256 hash, ulong height, DateTime time, DateTime medianTime, Sha256? previousBlockHash, Sha256? nextBlockHash,
                            Sha256 merkleRoot, IEnumerable<Sha256> txHashes)
        {
            if (height < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(height), $"{nameof(height)} must have a value greater than 0.");
            }

            if (time.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(time), $"{nameof(time)} must have a valid value.");
            }

            if (medianTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(medianTime), $"{nameof(medianTime)} must have a valid value.");
            }

            Hash = hash;
            Height = height;
            Time = time;
            MedianTime = medianTime;
            PreviousBlockHash = previousBlockHash;
            NextBlockHash = nextBlockHash;
            MerkleRoot = merkleRoot;
            TxHashes = txHashes;
        }

        public Sha256 Hash { get; }
        public ulong Height { get; }
        public DateTime Time { get; }
        public DateTime MedianTime { get; }
        public Sha256? PreviousBlockHash { get; }
        public Sha256? NextBlockHash { get; }
        public Sha256 MerkleRoot { get; }
        public IEnumerable<Sha256> TxHashes { get; }

        public bool IsNewYearFromPrevious(DateTime previousBlockMedianTime)
        {
            return previousBlockMedianTime.Year < MedianTime.Year;
        }

        public bool IsNewMonthFromPrevious(DateTime previousBlockMedianTime)
        {
            return IsNewYearFromPrevious(previousBlockMedianTime) ||
                   previousBlockMedianTime.Month < MedianTime.Month;
        }

        public bool IsNewDayFromPrevious(DateTime previousBlockMedianTime)
        {
            return IsNewMonthFromPrevious(previousBlockMedianTime) ||
                   previousBlockMedianTime.Day < MedianTime.Day;
        }

        public bool IsNewHourFromPrevious(DateTime previousBlockMedianTime)
        {
            return IsNewDayFromPrevious(previousBlockMedianTime) ||
                   previousBlockMedianTime.Hour < MedianTime.Hour;
        }

        public bool IsNewMinuteFromPrevious(DateTime previousBlockMedianTime)
        {
            return IsNewHourFromPrevious(previousBlockMedianTime) ||
                   previousBlockMedianTime.Minute < MedianTime.Minute;
        }
    }
}
