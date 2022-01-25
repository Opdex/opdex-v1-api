using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;

public class VaultProposalsCursor : Cursor<(ulong, ulong)>
{
    public VaultProposalsCursor(HashSet<VaultProposalStatus> statuses, HashSet<VaultProposalType> types, SortDirectionType sortDirection, uint limit,
                                PagingDirection pagingDirection, (ulong, ulong) pointer)
        : base(sortDirection, limit, pagingDirection, pointer)
    {
        Statuses = statuses ?? new HashSet<VaultProposalStatus>();
        Types = types ?? new HashSet<VaultProposalType>();
    }

    public HashSet<VaultProposalStatus> Statuses { get; }
    public HashSet<VaultProposalType> Types { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
        var encodedPointer = Convert.ToBase64String(pointerBytes);

        var sb = new StringBuilder();
        sb.AppendFormat("direction:{0};limit:{1};paging:{2};", SortDirection, Limit, PagingDirection);
        foreach (var status in Statuses) sb.AppendFormat("status:{0};", status);
        foreach (var type in Types) sb.AppendFormat("type:{0};", type);
        sb.AppendFormat("pointer:{0};", encodedPointer);
        return sb.ToString();
    }

    /// <inheritdoc />
    public override Cursor<(ulong, ulong)> Turn(PagingDirection direction, (ulong, ulong) pointer)
    {
        if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
        if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical pointer.");

        return new VaultProposalsCursor(Statuses, Types, SortDirection, Limit, direction, pointer);
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

        TryGetCursorProperties<VaultProposalStatus>(values, "status", out var statuses);

        TryGetCursorProperties<VaultProposalType>(values, "type", out var types);

        if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

        if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

        if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

        if (!pointerEncoded.HasValue()) return false;

        if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

        if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

        try
        {
            cursor = new VaultProposalsCursor(statuses.ToHashSet(), types.ToHashSet(), direction, limit, paging, pointer);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    private static bool TryDecodePointer(string encoded, out (ulong, ulong) pointer)
    {
        pointer = default;

        if (!encoded.TryBase64Decode(out var decoded)) return false;

        var tupleParts = decoded.Replace("(", "").Replace(")", "").Split(',');
        if (tupleParts.Length != 2 || !ulong.TryParse(tupleParts[0], out var expiration) || !ulong.TryParse(tupleParts[1], out var publicId)) return false;

        pointer = (expiration, publicId);
        return true;
    }
}
