using Microsoft.AspNetCore.Mvc.ModelBinding;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests;

public sealed class SnapshotFilterParameters : FilterParameters<SnapshotCursor>
{
    /// <summary>
    /// The snapshot step interval.
    /// </summary>
    /// <example>1D</example>
    public Interval Interval { get; set; }

    /// <summary>
    /// Start time for which to retrieve snapshots.
    /// </summary>
    /// <example>2021-01-01T00:00:00Z</example>
    [BindRequired]
    public DateTime StartDateTime { get; set; }

    /// <summary>
    /// End time for which to retrieve snapshots.
    /// </summary>
    /// <example>2021-08-01T00:00:00Z</example>
    [BindRequired]
    public DateTime EndDateTime { get; set; }

    /// <summary>
    /// Number of results to return per page.
    /// </summary>
    /// <example>100</example>
    public override uint Limit { get => base.Limit; set => base.Limit = value; }

    /// <inheritdoc />
    protected override SnapshotCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new SnapshotCursor(Interval, StartDateTime, EndDateTime, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        _ = SnapshotCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}