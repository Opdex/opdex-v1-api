using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Models.UInt;
using System;
using System.Numerics;

namespace Opdex.Platform.Common.Extensions
{
    public static class MathExtensions
    {
        /// <summary>
        /// Calculates reward amounts based on volume and other metrics for providers and the market.
        /// </summary>
        /// <remarks>Market fees refer to either standard market fees or staking rewards</remarks>
        /// <param name="volumeUsd">The volume in USD.</param>
        /// <param name="stakingWeight">The staking weight of the pool or market.</param>
        /// <param name="stakingEnabled">If staking is enabled in the market or pool.</param>
        /// <param name="transactionFee">The transaction fee as in contract (uint).</param>
        /// <param name="marketFeeEnabled">If the market has fees enabled, true in staking markets, either in standard.</param>
        /// <returns>Tuple of provider and market USD rewards.</returns>
        public static (decimal providerUsd, decimal marketUsd) VolumeBasedRewards(decimal volumeUsd, UInt256 stakingWeight, bool stakingEnabled,
                                                                                  uint transactionFee, bool marketFeeEnabled)
        {
            var fee = transactionFee / (decimal)1000;
            var totalRewards = Math.Round(volumeUsd * fee, 2, MidpointRounding.AwayFromZero);
            decimal providerUsd = 0m;
            decimal marketUsd = 0m;

            // Zero staking weight, all fees to providers
            var emptyStakingPool = stakingEnabled && stakingWeight == UInt256.Zero;

            if (emptyStakingPool || !marketFeeEnabled)
            {
                providerUsd = totalRewards;
            }
            else // Split rewards
            {
                marketUsd = Math.Round(totalRewards / 6, 2, MidpointRounding.AwayFromZero); // 1/6
                providerUsd = totalRewards - marketUsd; // 5/6
            }

            return (providerUsd, marketUsd);
        }

        /// <summary>
        /// Returns the total fiat amount of a number of tokens with a known price per token.
        /// </summary>
        /// <param name="tokenAmount">The total number of tokens.</param>
        /// <param name="fiatPerToken">The fiat price per full token.</param>
        /// <param name="tokenSats">The number of satoshis per token.</param>
        /// <returns>The fiat amount as a decimal of total amount of tokens provided.</returns>
        public static decimal TotalFiat(UInt256 tokenAmount, decimal fiatPerToken, ulong tokenSats)
        {
            if (tokenAmount == UInt256.Zero)
            {
                return 0.00m;
            }

            const ulong fiatOffset = FiatConstants.Usd.Offset;

            // Offset the fiat decimal
            var fiatPerWithOffsetBigInt = UInt256.Parse(Math.Floor(fiatPerToken * fiatOffset).ToString());

            // Attempt to parse token * fiatPer / sats as the fiat price including the fiat offset
            if (!decimal.TryParse((tokenAmount * fiatPerWithOffsetBigInt / tokenSats).ToString(), out var fiatWithOffsetDecimal))
            {
                throw new Exception("Unable to parse fiat decimal including offset.");
            }

            // Remove the fiatOffset and round back to a decimal
            return Math.Round(fiatWithOffsetDecimal / fiatOffset, FiatConstants.Usd.Decimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Returns the total fiat amount of a number of tokens with a known price per token.
        /// </summary>
        /// <param name="tokenAmount">The total number of tokens.</param>
        /// <param name="fiatPerToken">The fiat price per full token.</param>
        /// <param name="tokenSats">The number of satoshis per token.</param>
        /// <returns>The fiat amount as a decimal of total amount of tokens provided.</returns>
        public static decimal TotalFiat(ulong tokenAmount, decimal fiatPerToken, ulong tokenSats)
        {
            return TotalFiat((UInt256)tokenAmount, fiatPerToken, tokenSats);
        }

        /// <summary>
        /// Returns the fiat amount per token.
        /// </summary>
        /// <param name="tokenAmount">The total number or supply of tokens.</param>
        /// <param name="fiatAmount">The total fiat amount of all tokens combined.</param>
        /// <param name="tokenSats">The amount of satoshis in the token.</param>
        /// <returns>Decimal representing fiat value per token.</returns>
        public static decimal FiatPerToken(UInt256 tokenAmount, decimal fiatAmount, ulong tokenSats)
        {
            if (tokenAmount == UInt256.Zero)
            {
                return 0.00m;
            }

            const ulong fiatOffset = FiatConstants.Usd.Offset;

            // Offset the fiat decimal
            var fiatWithOffsetBigInt = UInt256.Parse(Math.Floor(fiatAmount * fiatOffset).ToString());

            // Attempt to parse fiatWithOffset * tokenSats / tokenAmount
            if (!decimal.TryParse((fiatWithOffsetBigInt * tokenSats / tokenAmount).ToString(), out var fiatWithOffsetDecimal))
            {
                throw new Exception("Unable to parse fiat decimal including offset.");
            }

            // Remove the fiat offset and round back to decimal
            return Math.Round(fiatWithOffsetDecimal / fiatOffset, FiatConstants.Usd.Decimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Get the percentage change difference of two string values as satoshis.
        /// </summary>
        /// <param name="current">The current amount.</param>
        /// <param name="previous">The previous amount.</param>
        /// <param name="tokenSats">The amount of sats per token.</param>
        /// <returns>Decimal of percent change</returns>
        public static decimal PercentChange(UInt256 current, UInt256 previous, ulong tokenSats)
        {
            if (previous == UInt256.Zero) return 0m;

            // Convert to Big Integer
            BigInteger currentBigInt = current;
            BigInteger previousBigInt = previous;

            // Apply offset of tokenSats
            var currentOffset = currentBigInt * tokenSats;
            var previousOffset = previousBigInt * tokenSats;

            // Find the percentage change with offset
            var change = (currentOffset - previousOffset) / previousBigInt * 100;

            // Try Parse
            if (!decimal.TryParse(change.ToString(), out var changeDecimalWithOffset))
            {
                throw new Exception("Unable to parse change decimal including offset.");
            }

            // Return percentage change without offset, rounded 2 places
            return Math.Round(changeDecimalWithOffset / tokenSats, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Get the percentage change difference of two decimal values.
        /// </summary>
        /// <param name="current">The current amount.</param>
        /// <param name="previous">The previous amount to compare to.</param>
        /// <returns>Percentage changed as decimal</returns>
        public static decimal PercentChange(decimal current, decimal previous)
        {
            if (previous <= 0)
            {
                return 0.00m;
            }

            var usdDailyChange = (current - previous) / previous * 100;

            return Math.Round(usdDailyChange, 2, MidpointRounding.AwayFromZero);
        }
    }
}
