using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;

public class VaultProposalPledgesCursor : Cursor<ulong>
{
    public VaultProposalPledgesCursor(Address pledger, bool includeZeroBalances, SortDirectionType sortDirection, uint limit, PagingDirection pagingDirection, ulong pointer)
        : base(sortDirection, limit, pagingDirection, pointer, DefaultLimit, DefaultMaxLimit, DefaultSortDirectionType)
    {
        Pledger = pledger;
        IncludeZeroBalances = includeZeroBalances;
    }

    public Address Pledger { get; }
    public bool IncludeZeroBalances { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
        var encodedPointer = Convert.ToBase64String(pointerBytes);
        return $"pledger:{Pledger};includeZeroBalances:{IncludeZeroBalances};direction:{SortDirection};limit:{Limit};paging:{PagingDirection};pointer:{encodedPointer};";
    }

    /// <inheritdoc />
    public override Cursor<ulong> Turn(PagingDirection direction, ulong pointer)
    {
        if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
        if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical id.");

        return new VaultProposalPledgesCursor(Pledger, IncludeZeroBalances, SortDirection, Limit, direction, pointer);
    }

    /// <inheritdoc />
    protected override bool ValidatePointer(ulong pointer) => pointer >= 0 && base.ValidatePointer(pointer);

    /// <summary>
    /// Parses a stringified version of the cursor
    /// </summary>
    /// <param name="raw">Stringified cursor</param>
    /// <param name="cursor">Parsed cursor</param>
    /// <returns>True if the value could be parsed, otherwise false</returns>
    public static bool TryParse(string raw, out VaultProposalPledgesCursor cursor)
    {
        cursor = null;

        if (raw is null) return false;

        var values = ToDictionary(raw);

        TryGetCursorProperty<Address>(values, "pledger", out var pledger);

        if (!TryGetCursorProperty<bool>(values, "includeZeroBalances", out var includeZeroBalances)) return false;

        if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

        if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

        if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

        if (!pointerEncoded.HasValue()) return false;

        if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

        if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

        try
        {
            cursor = new VaultProposalPledgesCursor(pledger, includeZeroBalances, direction, limit, paging, pointer);
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
