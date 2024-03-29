using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Votes;

public class VaultProposalVotesCursor : Cursor<ulong>
{
    public VaultProposalVotesCursor(ulong proposalId, Address voter, bool includeZeroBalances, SortDirectionType sortDirection, uint limit, PagingDirection pagingDirection, ulong pointer)
        : base(sortDirection, limit, pagingDirection, pointer)
    {
        ProposalId = proposalId;
        Voter = voter;
        IncludeZeroBalances = includeZeroBalances;
    }

    public ulong ProposalId { get; }
    public Address Voter { get; }
    public bool IncludeZeroBalances { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
        var encodedPointer = Convert.ToBase64String(pointerBytes);
        return $"proposalId:{ProposalId};voter:{Voter};includeZeroBalances:{IncludeZeroBalances};direction:{SortDirection};limit:{Limit};paging:{PagingDirection};pointer:{encodedPointer};";
    }

    /// <inheritdoc />
    public override Cursor<ulong> Turn(PagingDirection direction, ulong pointer)
    {
        if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
        if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical id.");

        return new VaultProposalVotesCursor(ProposalId, Voter, IncludeZeroBalances, SortDirection, Limit, direction, pointer);
    }

    /// <summary>
    /// Parses a stringified version of the cursor
    /// </summary>
    /// <param name="raw">Stringified cursor</param>
    /// <param name="cursor">Parsed cursor</param>
    /// <returns>True if the value could be parsed, otherwise false</returns>
    public static bool TryParse(string raw, out VaultProposalVotesCursor cursor)
    {
        cursor = null;

        if (raw is null) return false;

        var values = ToDictionary(raw);

        if (!TryGetCursorProperty<ulong>(values, "proposalId", out var proposalId)) return false;

        TryGetCursorProperty<Address>(values, "voter", out var voter);

        if (!TryGetCursorProperty<bool>(values, "includeZeroBalances", out var includeZeroBalances)) return false;

        if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

        if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

        if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

        if (!pointerEncoded.HasValue()) return false;

        if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

        if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

        try
        {
            cursor = new VaultProposalVotesCursor(proposalId, voter, includeZeroBalances, direction, limit, paging, pointer);
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
