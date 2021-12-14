using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Proposals;

public class VaultProposalsCursor : Cursor<(ulong, ulong)>
{
    public VaultProposalsCursor(VaultProposalStatus status, VaultProposalType type, SortDirectionType sortDirection, uint limit, PagingDirection pagingDirection, (ulong, ulong) pointer)
        : base(sortDirection, limit, pagingDirection, pointer, DefaultLimit, DefaultMaxLimit, DefaultSortDirectionType)
    {
        Status = status;
        Type = type;
    }

    public VaultProposalStatus Status { get; }
    public VaultProposalType Type { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
        var encodedPointer = Convert.ToBase64String(pointerBytes);
        return $"status:{Status};type:{Type};direction:{SortDirection};limit:{Limit};paging:{PagingDirection};pointer:{encodedPointer};";
    }

    /// <inheritdoc />
    public override Cursor<(ulong, ulong)> Turn(PagingDirection direction, (ulong, ulong) pointer)
    {
        if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
        if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical pointer.");

        return new VaultProposalsCursor(Status, Type, SortDirection, Limit, direction, pointer);
    }

    /// <summary>
    /// Parses a stringified version of the cursor
    /// </summary>
    /// <param name="raw">Stringified cursor</param>
    /// <param name="cursor">Parsed cursor</param>
    /// <returns>True if the value could be parsed, otherwise false</returns>
    public static bool TryParse(string raw, out VaultProposalsCursor cursor)
    {
        cursor = null;

        if (raw is null) return false;

        var values = ToDictionary(raw);

        TryGetCursorProperty<VaultProposalStatus>(values, "status", out var status);

        TryGetCursorProperty<VaultProposalType>(values, "type", out var type);

        if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

        if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

        if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

        if (!pointerEncoded.HasValue()) return false;

        if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

        if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

        try
        {
            cursor = new VaultProposalsCursor(status, type, direction, limit, paging, pointer);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    private static bool TryDecodePointer(string encoded, out (ulong, ulong) pointer)
    {
        pointer = (default, default);

        if (!encoded.TryBase64Decode(out var decoded)) return false;

        var tupleParts = decoded.Replace("(", "").Replace(")", "").Split(',');

        if (tupleParts.Length != 2 || !ulong.TryParse(tupleParts[0], out var expiration) || !ulong.TryParse(tupleParts[1], out var identifier)) return false;

        pointer = (expiration, identifier);
        return true;
    }
}
