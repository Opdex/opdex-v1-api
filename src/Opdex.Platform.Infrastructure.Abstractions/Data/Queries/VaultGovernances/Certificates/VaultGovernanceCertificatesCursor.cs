using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;

public class VaultGovernanceCertificatesCursor : Cursor<ulong>
{
    public VaultGovernanceCertificatesCursor(Address holder, VaultCertificateStatusFilter status, SortDirectionType sortDirection, uint limit, PagingDirection pagingDirection, ulong pointer)
        : base(sortDirection, limit, pagingDirection, pointer)
    {
        Holder = holder;
        Status = status;
    }

    public Address Holder { get; }
    public VaultCertificateStatusFilter Status { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
        var encodedPointer = Convert.ToBase64String(pointerBytes);
        return $"holder:{Holder};status:{Status};direction:{SortDirection};limit:{Limit};paging:{PagingDirection};pointer:{encodedPointer};";
    }

    /// <inheritdoc />
    public override Cursor<ulong> Turn(PagingDirection direction, ulong pointer)
    {
        if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
        if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical id.");

        return new VaultGovernanceCertificatesCursor(Holder, Status, SortDirection, Limit, direction, pointer);
    }

    /// <summary>
    /// Parses a stringified version of the cursor
    /// </summary>
    /// <param name="raw">Stringified cursor</param>
    /// <param name="cursor">Parsed cursor</param>
    /// <returns>True if the value could be parsed, otherwise false</returns>
    public static bool TryParse(string raw, out VaultGovernanceCertificatesCursor cursor)
    {
        cursor = null;

        if (raw is null) return false;

        var values = ToDictionary(raw);

        TryGetCursorProperty<Address>(values, "holder", out var holder);

        if (!TryGetCursorProperty<VaultCertificateStatusFilter>(values, "status", out var status)) return false;

        if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

        if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

        if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

        if (!pointerEncoded.HasValue()) return false;

        if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

        if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

        try
        {
            cursor = new VaultGovernanceCertificatesCursor(holder, status, direction, limit, paging, pointer);
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
    All, Redeemed, Revoked
}
