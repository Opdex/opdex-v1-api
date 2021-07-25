using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers
{
    public abstract class EntryFilterQueryHandler<T, TK> : IRequestHandler<T, TK> where T : IRequest<TK>
    {
        public abstract Task<TK> Handle(T request, CancellationToken cancellationToken);

        /// <summary>
        /// Based on the number of returned records and the expected limit + 1 value, return null or a value of
        /// the index that should be removed from the final response to the client.
        /// </summary>
        /// <param name="pagingBackward">Flag indicating if this request is paging backward.</param>
        /// <param name="count">The number of returned records.</param>
        /// <param name="limitPlusOne">The number of the requests limit + 1.</param>
        /// <returns>Nullable integer, when a value is present, the index of the result set to remove.</returns>
        protected int? RemoveAtIndex(bool pagingBackward, int count, uint limitPlusOne)
        {
            // If there's the + 1 record only
            if (count < limitPlusOne) return null;

            // Moving backwards remove the first item else remove the last item
            return pagingBackward ? 0 : count - 1;
        }

        /// <summary>
        /// Builds and returns a CursorDto based on the query request and response, removing the additional result from the result set.
        /// </summary>
        /// <param name="results">Query results required for building the cursor.</param>
        /// <returns><see cref="CursorDto"/> including next and previous page cursors.</returns>
        protected CursorDto BuildCursorDto<TResult, TPointer>(IList<TResult> results, Cursor<TPointer> cursor, Func<TResult, TPointer> extractPointerExpression)
        {
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
            if (resultsCount == 0) return dto;

            // Gather first and last values of the response set to build cursors after the + 1 has been removed.
            var firstResultPointer = extractPointerExpression.Invoke(results[0]);
            var lastResultPointer = extractPointerExpression.Invoke(results[results.Count - 1]);

            // If there is an extra result, we can always create a next cursor
            if (resultsCount == limitPlusOne)
            {
                dto.Next = cursor.Turn(PagingDirection.Forward, lastResultPointer).ToString().Base64Encode();

                if (cursor.PagingDirection == PagingDirection.Backward)
                {
                    dto.Previous = cursor.Turn(PagingDirection.Backward, firstResultPointer).ToString().Base64Encode();
                }
                // We never want to create a previous cursor on the first request
                else if (cursor.PagingDirection == PagingDirection.Forward && !cursor.IsFirstRequest)
                {
                    dto.Previous = cursor.Turn(PagingDirection.Backward, firstResultPointer).ToString().Base64Encode();
                }
            }
            // If we have already retrieved up to the max number of results on the first request, don't return a cursor
            else if (!cursor.IsFirstRequest)
            {
                if (cursor.PagingDirection == PagingDirection.Backward)
                {
                    dto.Next = cursor.Turn(PagingDirection.Forward, lastResultPointer).ToString().Base64Encode();
                }

                if (cursor.PagingDirection == PagingDirection.Forward)
                {
                    dto.Previous = cursor.Turn(PagingDirection.Backward, firstResultPointer).ToString().Base64Encode();
                }
            }

            return dto;
        }
        private int? RemoveAtIndex(PagingDirection pagingDirection, int count, uint limitPlusOne)
        {
            if (count < limitPlusOne) return null;

            return pagingDirection == PagingDirection.Backward ? 0 : count - 1;
        }
    }
}
