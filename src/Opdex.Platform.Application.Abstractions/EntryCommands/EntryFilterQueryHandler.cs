using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Abstractions.EntryCommands
{
    public abstract class EntryFilterQueryHandler<T, TK> : IRequestHandler<T, TK> where T : EntryFilterQuery<TK>
    {
        protected readonly IMediator _mediator;

        protected EntryFilterQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public abstract Task<TK> Handle(T request, CancellationToken cancellationToken);

        protected (CursorDto cursorDto, int? removeAt) BuildCursorDto(bool previousHasValue, bool nextHasValue, int count, uint limitPlusOne,
                                                                      string defaultCursor, string firstCursorString, string lastCursorString)
        {
            var cursor = new CursorDto();
            int? removeAt = null;

            if (count == limitPlusOne)
            {
                // If the + 1 record is returned, there is always a next page
                cursor.Next = BuildCursor(defaultCursor, lastCursorString, true);

                if (previousHasValue) // Going backward
                {
                    // Moving backwards remove the first item (the +1)
                    removeAt = 0;

                    cursor.Previous = BuildCursor(defaultCursor, firstCursorString, false);

                }
                else // Going forward but maybe not only to the first page
                {
                    // Moving forwards remove the last item (the +1)
                    removeAt = count - 1;

                    // request.Next defined means we're past the first page moving onto the NEXT page.
                    // meaning, we'll have a previous page. (The one we're coming from)
                    if (nextHasValue)
                    {
                        cursor.Previous = BuildCursor(defaultCursor, firstCursorString, false);
                    }
                }
            }
            else // There was no +1, we've hit the end or beginning of our search
            {
                // going backward hitting the beginning
                if (previousHasValue)
                {
                    cursor.Next = BuildCursor(defaultCursor, lastCursorString, true);
                }

                // going forward hitting the end
                if (nextHasValue)
                {
                    cursor.Previous = BuildCursor(defaultCursor, firstCursorString, false);
                }
            }

            return (cursor, removeAt);
        }

        protected static string BuildCursorFromList(string key, List<string> values)
        {
            var sb = new StringBuilder();

            foreach (var value in values) sb.Append($"{key}:{value};");

            return sb.ToString();
        }

        private static string BuildCursor(string defaultCursor, string cursorValue, bool next)
        {
            var build = new StringBuilder(defaultCursor);

            var cursorType = next ? "next" : "previous";

            return build
                .Append($"{cursorType}:{cursorValue.Base64Encode()};")
                .ToString()
                .Base64Encode();
        }
    }
}
