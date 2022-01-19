using Microsoft.AspNetCore.Mvc.ModelBinding;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Markets;

public class MarketSnapshotFilterParameters : FilterParameters<SnapshotCursor>
{
    /// <summary>
    /// Start time for which to retrieve snapshots.
    /// </summary>
    [BindRequired]
    public DateTime StartDateTime { get; set; }

    /// <summary>
    /// End time for which to retrieve snapshots.
    /// </summary>
    [BindRequired]
    public DateTime EndDateTime { get; set; }

    public override uint Limit { get => base.Limit; set => base.Limit = value; }

    /// <inheritdoc />
    protected override SnapshotCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new SnapshotCursor(Interval.OneDay, StartDateTime, EndDateTime, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        SnapshotCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}