using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;

public class VaultCertificatesCursor : Cursor<ulong>
{
    public VaultCertificatesCursor(Address holder, HashSet<VaultCertificateStatusFilter> statuses, SortDirectionType sortDirection, uint limit, PagingDirection pagingDirection, ulong pointer)
        : base(sortDirection, limit, pagingDirection, pointer)
    {
        Holder = holder;
        Statuses = statuses ?? new HashSet<VaultCertificateStatusFilter>();
    }

    public Address Holder { get; }
    public HashSet<VaultCertificateStatusFilter> Statuses { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
        var encodedPointer = Convert.ToBase64String(pointerBytes);

        var sb = new StringBuilder();
        sb.AppendFormat("direction:{0};limit:{1};paging:{2};", SortDirection, Limit, PagingDirection);
        foreach (var status in Statuses) sb.AppendFormat("status:{0};", status);
        sb.AppendFormat("holder:{0};", Holder);
        sb.AppendFormat("pointer:{0};", encodedPointer);
        return sb.ToString();
    }

    /// <inheritdoc />
    public override Cursor<ulong> Turn(PagingDirection direction, ulong pointer)
    {
        if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
        if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical id.");

        return new VaultCertificatesCursor(Holder, Statuses, SortDirection, Limit, direction, pointer);
    }

    /// <summary>
    /// Parses a stringified version of the cursor
    /// </summary>
    /// <param name="raw">Stringified cursor</param>
    /// <param name="cursor">Parsed cursor</param>
    /// <returns>True if the value could be parsed, otherwise false</returns>
    public static bool TryParse(string raw, out VaultCertificatesCursor cursor)
    {
        cursor = null;

        if (raw is null) return false;

        var values = ToDictionary(raw);

        TryGetCursorProperty<Address>(values, "holder", out var holder);

        TryGetCursorProperties<VaultCertificateStatusFilter>(values, "status", out var statuses);

        if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

        if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

        if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

        if (!pointerEncoded.HasValue()) return false;

        if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

        if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

        try
        {
            cursor = new VaultCertificatesCursor(holder, statuses.ToHashSet(), direction, limit, paging, pointer);
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

public enum VaultCertificateStatusFilter
{
    Vesting = 1, Redeemed = 2, Revoked = 3
}
