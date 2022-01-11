using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

public class MarketsCursor : Cursor<(FixedDecimal, ulong)>
{
    public MarketsCursor(MarketType type, MarketOrderByType orderBy, SortDirectionType sortDirection, uint limit, PagingDirection pagingDirection, (FixedDecimal, ulong) pointer)
        : base(sortDirection, limit, pagingDirection, pointer)
    {
        Type = type;
        OrderBy = orderBy;
    }

    public MarketType Type { get; }
    public MarketOrderByType OrderBy { get; }

    /// <inheritdoc />
    public override Cursor<(FixedDecimal, ulong)> Turn(PagingDirection direction, (FixedDecimal, ulong) pointer)
    {
        if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
        if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical pointer.");

        return new MarketsCursor(Type, OrderBy, SortDirection, Limit, direction, pointer);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
        var encodedPointer = Convert.ToBase64String(pointerBytes);

        var sb = new StringBuilder();
        sb.AppendFormat("direction:{0};limit:{1};paging:{2};", SortDirection, Limit, PagingDirection);
        sb.AppendFormat("type:{0};", Type);
        sb.AppendFormat("orderBy:{0};", OrderBy);
        sb.AppendFormat("pointer:{0};", encodedPointer);
        return sb.ToString();
    }
    
    /// <summary>
    /// Parses a stringified version of the cursor
    /// </summary>
    /// <param name="raw">Stringified cursor</param>
    /// <param name="cursor">Parsed cursor</param>
    /// <returns>True if the value could be parsed, otherwise false</returns>
    public static bool TryParse(string raw, out MarketsCursor cursor)
    {
        cursor = null;

        if (raw is null) return false;

        var values = ToDictionary(raw);

        if (!TryGetCursorProperty<MarketType>(values, "type", out var type)) return false;

        if (!TryGetCursorProperty<MarketOrderByType>(values, "orderBy", out var orderBy)) return false;

        if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

        if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

        if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

        if (!pointerEncoded.HasValue()) return false;

        if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

        if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

        try
        {
            cursor = new MarketsCursor(type, orderBy, direction, limit, paging, pointer);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    private static bool TryDecodePointer(string encoded, out (FixedDecimal, ulong) pointer)
    {
        pointer = (FixedDecimal.Zero, 0UL);

        if (!encoded.TryBase64Decode(out var decoded)) return false;

        var tupleParts = decoded.Replace("(", "").Replace(")", "").Split(',');

        if (tupleParts.Length != 2
            || !FixedDecimal.TryParse(tupleParts[0], out var firstOrder)
            || !ulong.TryParse(tupleParts[1], out var identifier)) return false;

        pointer = (firstOrder, identifier);

        return true;
    }
}
