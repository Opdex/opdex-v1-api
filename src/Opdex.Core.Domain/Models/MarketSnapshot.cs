using System;

namespace Opdex.Core.Domain.Models
{
    public class MarketSnapshot
    {
        public MarketSnapshot(long tokenCount, long pairCount, long dailyTransactionCount, decimal crsPrice, decimal liquidity,
            decimal dailyFees, decimal dailyVolume, ulong block)
        {
            if (tokenCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenCount));
            }
            
            if (pairCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pairCount));
            }
            
            if (dailyTransactionCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(dailyTransactionCount));
            }
            
            if (crsPrice < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(crsPrice));
            }
            
            if (liquidity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidity));
            }
            
            if (dailyFees < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(dailyFees));
            }
            
            if (dailyVolume < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(dailyVolume));
            }
            
            if (block < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(block));
            }

            TokenCount = tokenCount;
            PairCount = pairCount;
            DailyTransactionCount = dailyTransactionCount;
            CrsPrice = crsPrice;
            Liquidity = liquidity;
            DailyFees = dailyFees;
            DailyVolume = dailyVolume;
            Block = block;
        }
        
        public MarketSnapshot(long id, long tokenCount, long pairCount, long dailyTransactionCount, decimal crsPrice, decimal liquidity,
            decimal dailyFees, decimal dailyVolume, ulong block)
        {
            Id = id;
            TokenCount = tokenCount;
            PairCount = pairCount;
            DailyTransactionCount = dailyTransactionCount;
            CrsPrice = crsPrice;
            Liquidity = liquidity;
            DailyFees = dailyFees;
            DailyVolume = dailyVolume;
            Block = block;
        }
        
        public long Id { get; }
        public long TokenCount { get; }
        public long PairCount { get; }
        public long DailyTransactionCount { get; }
        public decimal CrsPrice { get; }
        public decimal Liquidity { get; }
        public decimal DailyFees { get; }
        public decimal DailyVolume { get; }
        public ulong Block { get; }
    }
}