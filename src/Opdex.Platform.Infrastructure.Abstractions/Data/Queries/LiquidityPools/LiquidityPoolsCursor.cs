using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools
{
    public class LiquidityPoolsCursor : Cursor<(string, ulong)>
    {
        public LiquidityPoolsCursor(string keyword, IEnumerable<Address> markets, IEnumerable<Address> pools, IEnumerable<Address> tokens, LiquidityPoolStakingStatusFilter stakingFilter,
                                    LiquidityPoolNominationStatusFilter nominationFilter, LiquidityPoolMiningStatusFilter miningFilter,
                                    LiquidityPoolOrderByType orderBy, SortDirectionType sortDirection, uint limit, PagingDirection pagingDirection,
                                    (string, ulong) pointer)
            : base(sortDirection, limit, pagingDirection, pointer)
        {
            Keyword = keyword;
            Markets = markets ?? Enumerable.Empty<Address>();
            LiquidityPools = pools ?? Enumerable.Empty<Address>();
            Tokens = tokens ?? Enumerable.Empty<Address>();
            StakingFilter = stakingFilter;
            NominationFilter = nominationFilter;
            MiningFilter = miningFilter;
            OrderBy = orderBy;
        }

        /// <summary>
        /// Constructor specifically for queries where all market pools are needed
        /// </summary>
        /// <remarks>
        /// This is to fill existing snapshot functionality. Improvements can be made to include pagination
        /// rather than select all, however, current pagination is done after assemblers use resources to build DTO models
        /// and may be counter productive at the moment without further refactoring.
        /// </remarks>
        /// <param name="market">The market to select all liquidity pools for.</param>
        public LiquidityPoolsCursor(Address market)
            : base(SortDirectionType.ASC, uint.MaxValue, PagingDirection.Forward, default, uint.MaxValue, uint.MaxValue)
        {
            Markets = new List<Address> { market };
            LiquidityPools = Enumerable.Empty<Address>();
            Tokens = Enumerable.Empty<Address>();
        }

        /// <summary>
        /// A generic keyword search against liquidity pool addresses and names.
        /// </summary>
        public string Keyword { get; }

        /// <summary>
        /// Markets to search liquidity pools within.
        /// </summary>
        public IEnumerable<Address> Markets { get; }

        /// <summary>
        /// Liquidity pools to filter specifically for.
        /// </summary>
        public IEnumerable<Address> LiquidityPools { get; }

        /// <summary>
        /// Tokens to filter specifically for.
        /// </summary>
        public IEnumerable<Address> Tokens { get; }

        /// <summary>
        /// Staking status filter, default ignores filter.
        /// </summary>
        public LiquidityPoolStakingStatusFilter StakingFilter { get; }

        /// <summary>
        /// Nomination status filter, default ignores filter.
        /// </summary>
        public LiquidityPoolNominationStatusFilter NominationFilter { get; }

        /// <summary>
        /// Mining status filter, default ignores filter.
        /// </summary>
        public LiquidityPoolMiningStatusFilter MiningFilter { get; }

        /// <summary>
        /// The order to sort records by.
        /// </summary>
        public LiquidityPoolOrderByType OrderBy { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
            var encodedPointer = Convert.ToBase64String(pointerBytes);

            var sb = new StringBuilder();
            sb.AppendFormat("direction:{0};limit:{1};paging:{2};", SortDirection, Limit, PagingDirection);
            foreach (var pool in LiquidityPools) sb.AppendFormat("liquidityPools:{0};", pool);
            foreach (var token in Tokens) sb.AppendFormat("tokens:{0};", token);
            foreach (var market in Markets) sb.AppendFormat("markets:{0};", market);
            sb.AppendFormat("keyword:{0};", Keyword);
            sb.AppendFormat("stakingFilter:{0};", StakingFilter);
            sb.AppendFormat("nominationFilter:{0};", NominationFilter);
            sb.AppendFormat("miningFilter:{0};", MiningFilter);
            sb.AppendFormat("orderBy:{0};", OrderBy);
            sb.AppendFormat("pointer:{0};", encodedPointer);
            return sb.ToString();
        }

        /// <inheritdoc />
        public override Cursor<(string, ulong)> Turn(PagingDirection direction, (string, ulong) pointer)
        {
            if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
            if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical pointer.");

            return new LiquidityPoolsCursor(Keyword, Markets, LiquidityPools, Tokens, StakingFilter, NominationFilter, MiningFilter,
                                            OrderBy, SortDirection, Limit, direction, pointer);
        }

        /// <summary>
        /// Parses a stringified version of the cursor
        /// </summary>
        /// <param name="raw">Stringified cursor</param>
        /// <param name="cursor">Parsed cursor</param>
        /// <returns>True if the value could be parsed, otherwise false</returns>
        public static bool TryParse(string raw, out LiquidityPoolsCursor cursor)
        {
            cursor = null;

            if (raw is null) return false;

            var values = ToDictionary(raw);

            TryGetCursorProperty<string>(values, "keyword", out var keyword);

            TryGetCursorProperties<Address>(values, "markets", out var markets);

            TryGetCursorProperty<LiquidityPoolOrderByType>(values, "orderBy", out var orderBy);

            TryGetCursorProperties<Address>(values, "tokens", out var tokens);

            TryGetCursorProperties<Address>(values, "liquidityPools", out var pools);

            TryGetCursorProperty<LiquidityPoolStakingStatusFilter>(values, "stakingFilter", out var stakingFilter);

            TryGetCursorProperty<LiquidityPoolNominationStatusFilter>(values, "nominationFilter", out var nominationFilter);

            TryGetCursorProperty<LiquidityPoolMiningStatusFilter>(values, "miningFilter", out var miningFilter);

            if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

            if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

            if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

            if (!pointerEncoded.HasValue()) return false;

            if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

            if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

            try
            {
                cursor = new LiquidityPoolsCursor(keyword, markets, pools, tokens, stakingFilter, nominationFilter, miningFilter,
                                                  orderBy, direction, limit, paging, pointer);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool TryDecodePointer(string encoded, out (string, ulong) pointer)
        {
            pointer = (string.Empty, 0);

            if (!encoded.TryBase64Decode(out var decoded)) return false;

            var tupleParts = decoded.Replace("(", "").Replace(")", "").Split(',');

            if (tupleParts.Length != 2 || !ulong.TryParse(tupleParts[1], out var ulongValue))
            {
                return false;
            }

            pointer = (tupleParts[0], ulongValue);

            return true;
        }
    }

    /// <summary>
    /// Options to filter liquidity pools by according to their mining status.
    /// </summary>
    public enum LiquidityPoolMiningStatusFilter
    {
        /// <summary>
        /// Retrieve any liquidity pool not determined by mining status.
        /// </summary>
        Any = 0,

        /// <summary>
        /// Mining enabled filter.
        /// </summary>
        Enabled = 1,

        /// <summary>
        /// Mining disabled filter.
        /// </summary>
        Disabled = 2
    }

    /// <summary>
    /// Options to filter liquidity pools by according to their nomination status.
    /// </summary>
    public enum LiquidityPoolNominationStatusFilter
    {
        /// <summary>
        /// Retrieve any liquidity pool not determined by nomination status.
        /// </summary>
        Any = 0,

        /// <summary>
        /// Nominated for liquidity mining filter.
        /// </summary>
        Nominated = 1,

        /// <summary>
        /// Not nominated for liquidity mining filter.
        /// </summary>
        NonNominated = 2
    }

    /// <summary>
    /// Options to filter liquidity pools by according to their staking status.
    /// </summary>
    public enum LiquidityPoolStakingStatusFilter
    {
        /// <summary>
        /// Retrieve any liquidity pool not determined by staking status.
        /// </summary>
        Any = 0,

        /// <summary>
        /// Staking enabled filter.
        /// </summary>
        Enabled = 1,

        /// <summary>
        /// Staking disabled filter.
        /// </summary>
        Disabled = 2
    }

    /// <summary>
    /// The order in which to return liquidity pool records by.
    /// </summary>
    public enum LiquidityPoolOrderByType
    {
        /// <summary>
        /// The default status of any is ignored in filter.
        /// </summary>
        Any = 0,

        /// <summary>
        /// Order results by liquidity locked amounts.
        /// </summary>
        Liquidity = 1,

        /// <summary>
        /// Order results by daily volume.
        /// </summary>
        Volume = 2,

        /// <summary>
        /// Order results by locked staking weight.
        /// </summary>
        StakingWeight = 3,

        /// <summary>
        /// Order results alphabetically by name.
        /// </summary>
        Name = 4
    }
}
