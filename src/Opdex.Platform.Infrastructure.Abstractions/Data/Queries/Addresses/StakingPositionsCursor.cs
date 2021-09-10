using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses
{
    public class StakingPositionsCursor : Cursor<long>
    {
        public StakingPositionsCursor(IEnumerable<Address> liquidityPools, bool includeZeroAmounts, SortDirectionType sortDirection, uint limit, PagingDirection pagingDirection, long pointer)
            : base(sortDirection, limit, pagingDirection, pointer)
        {
            LiquidityPools = liquidityPools ?? Enumerable.Empty<Address>();
            IncludeZeroAmounts = includeZeroAmounts;
        }

        public IEnumerable<Address> LiquidityPools { get; }
        public bool IncludeZeroAmounts { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
            var encodedPointer = Convert.ToBase64String(pointerBytes);

            var sb = new StringBuilder();
            sb.AppendFormat("direction:{0};limit:{1};paging:{2};", SortDirection, Limit, PagingDirection);
            foreach (var pool in LiquidityPools) sb.AppendFormat("liquidityPools:{0};", pool);
            sb.AppendFormat("includeZeroAmounts:{0};", IncludeZeroAmounts);
            sb.AppendFormat("pointer:{0};", encodedPointer);
            return sb.ToString();
        }

        /// <inheritdoc />
        public override Cursor<long> Turn(PagingDirection direction, long pointer)
        {
            if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
            if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical id.");

            return new StakingPositionsCursor(LiquidityPools, IncludeZeroAmounts, SortDirection, Limit, direction, pointer);
        }

        /// <inheritdoc />
        protected override bool ValidatePointer(long pointer) => pointer >= 0 && base.ValidatePointer(pointer);

        /// <summary>
        /// Parses a stringified version of the cursor
        /// </summary>
        /// <param name="raw">Stringified cursor</param>
        /// <param name="cursor">Parsed cursor</param>
        /// <returns>True if the value could be parsed, otherwise false</returns>
        public static bool TryParse(string raw, out StakingPositionsCursor cursor)
        {
            cursor = null;

            if (raw is null) return false;

            var values = ToDictionary(raw);

            TryGetCursorProperties<string>(values, "liquidityPools", out var liquidityPools);

            if (!TryGetCursorProperty<bool>(values, "includeZeroAmounts", out var includeZeroAmounts)) return false;

            if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

            if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

            if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

            if (!pointerEncoded.HasValue()) return false;

            if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

            if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

            try
            {
                cursor = new StakingPositionsCursor(liquidityPools.Select(pool => new Address(pool)), includeZeroAmounts, direction, limit, paging, pointer);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool TryDecodePointer(string encoded, out long pointer)
        {
            pointer = 0;

            if (!Base64Extensions.TryBase64Decode(encoded, out var decoded) || !long.TryParse(decoded, out var result)) return false;

            pointer = result;
            return true;
        }
    }
}
