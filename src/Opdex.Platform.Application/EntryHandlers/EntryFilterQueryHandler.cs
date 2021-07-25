using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries;
using Opdex.Platform.Application.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers
{
    public abstract class EntryFilterQueryHandler<T, TK> : IRequestHandler<T, TK> where T : EntryFilterQuery<TK>
    {
        protected readonly IMediator _mediator;

        protected EntryFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

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
        /// Builds and returns a CursorDto based on the query request and response.
        /// </summary>
        /// <param name="pagingBackward">Flag describing if the request is for the previous page.</param>
        /// <param name="pagingForward">Flag describing if the request is for the next page.</param>
        /// <param name="recordsFound">The number of records returned from the request (Maximum is limit + 1)</param>
        /// <param name="limitPlusOne">The value of the requests limit + 1</param>
        /// <param name="currentCursor">The base query cursor, unique per request, values will be appended to this current base.</param>
        /// <param name="firstRecordCursorString">The first records cursor values as a string in the list of returned records.</param>
        /// <param name="lastRecordCursorString">The last records cursor values as a string in the list of returned records.</param>
        /// <returns><see cref="CursorDto"/> including next and previous page cursors.</returns>
        protected CursorDto BuildCursorDto(bool pagingBackward, bool pagingForward, int recordsFound, uint limitPlusOne,
                                           string currentCursor, string firstRecordCursorString, string lastRecordCursorString)
        {
            var cursor = new CursorDto();

            // Limit + 1 returned, there is more than 1 page of results
            if (recordsFound == limitPlusOne)
            {
                // If the + 1 record is returned, there is always a next page
                cursor.SetNextCursor(currentCursor, lastRecordCursorString);

                if (pagingBackward)
                {
                    // Paging backward with + 1 record returned, there is a previous page
                    cursor.SetPreviousCursor(currentCursor, firstRecordCursorString);
                }
                else if (pagingForward)
                {
                    // Paging forward with + 1 record returned, there is a next page
                    cursor.SetPreviousCursor(currentCursor, firstRecordCursorString);
                }
            }
            // There was no +1, we've hit the end or beginning of our search
            else
            {
                if (pagingBackward)
                {
                    // Paging backward hitting the beginning, there is a next page
                    cursor.SetNextCursor(currentCursor, lastRecordCursorString);
                }

                if (pagingForward)
                {
                    // Paging forward hitting the end, there is a previous page
                    cursor.SetPreviousCursor(currentCursor, firstRecordCursorString);
                }
            }

            return cursor;
        }

        /// <summary>
        /// Builds and a returns a colon/semi-colon delimited cursor string. (e.g. "contracts:<one>;contracts:<two>;")
        /// </summary>
        /// <param name="key">They key for setting values.</param>
        /// <param name="values">List of values to set.</param>
        /// <returns>Built cursor string</returns>
        protected static string BuildCursorFromList(string key, IEnumerable<string> values)
        {
            var sb = new StringBuilder();

            foreach (var value in values) sb.Append($"{key}:{value};");

            return sb.ToString();
        }
    }
}
