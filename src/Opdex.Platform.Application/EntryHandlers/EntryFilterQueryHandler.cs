using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers;

public abstract class EntryFilterQueryHandler<T, TK> : IRequestHandler<T, TK> where T : IRequest<TK>
{
    private readonly ILogger<EntryFilterQueryHandler<T, TK>> _logger;

    protected EntryFilterQueryHandler(ILogger<EntryFilterQueryHandler<T, TK>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public abstract Task<TK> Handle(T request, CancellationToken cancellationToken);

    /// <summary>
    /// Builds and returns a CursorDto based on the query request and response, removing the additional result from the result set.
    /// </summary>
    /// <param name="results">Query results required for building the cursor.</param>
    /// <param name="cursor">Current request cursor.</param>
    /// <param name="pointerSelector">Expression to select the pointer from the result.</param>
    /// <returns><see cref="CursorDto"/> including next and previous page cursors.</returns>
    protected CursorDto BuildCursorDto<TResult, TPointer>(IList<TResult> results, Cursor<TPointer> cursor, Func<TResult, TPointer> pointerSelector)
        where TPointer : IEquatable<TPointer>
    {
        _logger.LogTrace("Attempting to build next and previous cursor");

        // The count can change if we remove the + 1 record, we want the original
        var resultsCount = results.Count;
        var limitPlusOne = cursor.Limit + 1;

        // Remove the + 1 value if necessary
        var removeAtIndex = RemoveAtIndex(cursor.PagingDirection, resultsCount, limitPlusOne);
        if (removeAtIndex.HasValue)
        {
            results.RemoveAt(removeAtIndex.Value);
        }

        var dto = new CursorDto();

        // With no results there can be no pagination
        if (resultsCount == 0)
        {
            _logger.LogTrace("Cursors not built as there was no results");
            return dto;
        }

        // Gather first and last values of the response set to build cursors after the + 1 has been removed.
        var firstResultPointer = pointerSelector.Invoke(results[0]);
        var lastResultPointer = pointerSelector.Invoke(results[^1]);

        // If there is an extra result, we can always create a next cursor
        if (resultsCount == limitPlusOne)
        {
            _logger.LogDebug("Building next cursor from pointer {Pointer}", lastResultPointer);
            dto.Next = cursor.Turn(PagingDirection.Forward, lastResultPointer).ToString().Base64Encode();

            // We never want to create a previous cursor on the first request
            if (cursor.IsFirstRequest)
            {
                return dto;
            }

            _logger.LogDebug("Building previous cursor from pointer {Pointer}", firstResultPointer);
            dto.Previous = cursor.Turn(PagingDirection.Backward, firstResultPointer).ToString().Base64Encode();
        }

        // If we have already retrieved up to the max number of results on the first request, don't return a cursor
        else if (!cursor.IsFirstRequest)
        {
            switch (cursor.PagingDirection)
            {
                case PagingDirection.Backward:
                    _logger.LogDebug("Building next cursor from pointer {Pointer}", lastResultPointer);
                    dto.Next = cursor.Turn(PagingDirection.Forward, lastResultPointer).ToString().Base64Encode();
                    break;
                case PagingDirection.Forward:
                    _logger.LogDebug("Building previous cursor from pointer {Pointer}", firstResultPointer);
                    dto.Previous = cursor.Turn(PagingDirection.Backward, firstResultPointer).ToString().Base64Encode();
                    break;
            }
        }

        return dto;
    }

    private static int? RemoveAtIndex(PagingDirection pagingDirection, int count, uint limitPlusOne)
    {
        if (count < limitPlusOne) return null;

        return pagingDirection == PagingDirection.Backward ? 0 : count - 1;
    }
}
