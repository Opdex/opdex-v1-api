using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;

public class MiningPoolsCursor : Cursor<ulong>
{
    public MiningPoolsCursor(IEnumerable<Address> liquidityPools, MiningStatusFilter miningStatus, SortDirectionType sortDirection, uint limit, PagingDirection pagingDirection, ulong pointer)
        : base(sortDirection, limit, pagingDirection, pointer)
    {
        LiquidityPools = liquidityPools ?? Enumerable.Empty<Address>();
        MiningStatus = miningStatus.IsValid() ? miningStatus : throw new ArgumentOutOfRangeException(nameof(miningStatus), "Mining status must be valid.");
    }

    /// <summary>
    /// Liquidity pools to filter mining pools by association
    /// </summary>
    public IEnumerable<Address> LiquidityPools { get; }

    /// <summary>
    /// Filter for mining status
    /// </summary>
    public MiningStatusFilter MiningStatus { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
        var encodedPointer = Convert.ToBase64String(pointerBytes);

        var sb = new StringBuilder();
        sb.AppendFormat("direction:{0};limit:{1};paging:{2};", SortDirection, Limit, PagingDirection);
        foreach (var pool in LiquidityPools) sb.AppendFormat("liquidityPools:{0};", pool);
        sb.AppendFormat("miningStatus:{0};", MiningStatus);
        sb.AppendFormat("pointer:{0};", encodedPointer);
        return sb.ToString();
    }

    /// <inheritdoc />
    public override Cursor<ulong> Turn(PagingDirection direction, ulong pointer)
    {
        if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
        if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical id.");

        return new MiningPoolsCursor(LiquidityPools, MiningStatus, SortDirection, Limit, direction, pointer);
    }

    /// <summary>
    /// Parses a stringified version of the cursor
    /// </summary>
    /// <param name="raw">Stringified cursor</param>
    /// <param name="cursor">Parsed cursor</param>
    /// <returns>True if the value could be parsed, otherwise false</returns>
    public static bool TryParse(string raw, out MiningPoolsCursor cursor)
    {
        cursor = null;

        if (raw is null) return false;

        var values = ToDictionary(raw);

        TryGetCursorProperties<Address>(values, "liquidityPools", out var liquidityPools);

        if (!TryGetCursorProperty<MiningStatusFilter>(values, "miningStatus", out var miningStatus)) return false;

        if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

        if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

        if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

        if (!pointerEncoded.HasValue()) return false;

        if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

        if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

        try
        {
            cursor = new MiningPoolsCursor(liquidityPools, miningStatus, direction, limit, paging, pointer);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    private static bool TryDecodePointer(string encoded, out ulong pointer)
    {
        pointer = 0;

        if (!Base64Extensions.TryBase64Decode(encoded, out var decoded) || !ulong.TryParse(decoded, out var result)) return false;

        pointer = result;
        return true;
    }
}

/// <summary>
/// Mining pool mining status filter
/// </summary>
public enum MiningStatusFilter
{
    Any = 0, Active = 1, Inactive = 2
}
