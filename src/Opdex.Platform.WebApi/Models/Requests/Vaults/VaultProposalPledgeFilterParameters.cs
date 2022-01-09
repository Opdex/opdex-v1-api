using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Pledges;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults;

public class VaultProposalPledgeFilterParameters : FilterParameters<VaultProposalPledgesCursor>
{
    /// <summary>
    /// Id of the proposal in the vault.
    /// </summary>
    /// <example>5</example>
    public ulong ProposalId { get; set; }

    /// <summary>
    /// Address of the pledger.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address Pledger { get; set; }

    /// <summary>
    /// Includes zero balances if true, otherwise filters out zero balances if false. Default false.
    /// </summary>
    /// <example>true</example>
    public bool IncludeZeroBalances { get; set; }

    protected override VaultProposalPledgesCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new VaultProposalPledgesCursor(ProposalId, Pledger, IncludeZeroBalances, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        _ = VaultProposalPledgesCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}
