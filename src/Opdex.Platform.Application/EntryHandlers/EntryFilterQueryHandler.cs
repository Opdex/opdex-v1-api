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

        protected int? RemoveAtIndex(bool previousHasValue, int count, uint limitPlusOne)
        {
            // If there's the + 1 record only
            if (count < limitPlusOne) return null;

            // Moving backwards remove the first item else remove the last item
            return previousHasValue ? 0 : count - 1;
        }

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

        protected static string BuildCursorFromList(string key, IEnumerable<string> values)
        {
            var sb = new StringBuilder();

            foreach (var value in values) sb.Append($"{key}:{value};");

            return sb.ToString();
        }
    }
}
