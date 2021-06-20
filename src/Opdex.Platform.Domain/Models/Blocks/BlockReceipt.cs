using System;
using System.Collections.Generic;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Blocks
{
    public class BlockReceipt
    {
        public BlockReceipt(string hash, ulong height, DateTime time, DateTime medianTime, string previousBlockHash, string nextBlockHash,
            string merkleRoot, IEnumerable<string> txHashes)
        {
            if (!hash.HasValue())
            {
                throw new ArgumentNullException(nameof(hash), $"{nameof(hash)} must have a value.");
            }

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

            if (!merkleRoot.HasValue())
            {
                throw new ArgumentNullException(nameof(merkleRoot), $"{nameof(merkleRoot)} must have a value.");
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

        public string Hash { get; }
        public ulong Height { get; }
        public DateTime Time { get; }
        public DateTime MedianTime { get; }
        public string PreviousBlockHash { get; }
        public string NextBlockHash { get; }
        public string MerkleRoot { get; }
        public IEnumerable<string> TxHashes { get; }

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