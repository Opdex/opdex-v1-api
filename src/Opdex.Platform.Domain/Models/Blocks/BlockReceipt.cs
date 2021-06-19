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

            if (!previousBlockHash.HasValue())
            {
                throw new ArgumentNullException(nameof(previousBlockHash), $"{nameof(previousBlockHash)} must have a value.");
            }

            if (!nextBlockHash.HasValue())
            {
                throw new ArgumentNullException(nameof(nextBlockHash), $"{nameof(nextBlockHash)} must have a value.");
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
    }
}