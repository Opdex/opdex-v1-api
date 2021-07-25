using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries
{
    public abstract class EntryFilterQuery<T> : IRequest<T>
    {
        protected EntryFilterQuery(SortDirectionType direction, uint limit, uint maxLimit, string next, string previous)
        {
            // Set the max limit
            MaximumLimit = maxLimit;

            // Only one or the other can have values
            if (next.HasValue() && previous.HasValue())
            {
                throw new ArgumentException("Next and previous cannot both have values.");
            }

            // Use encoded cursor if provided, ignoring other query strings
            if (next.HasValue() || previous.HasValue())
            {
                CursorProperties = previous.HasValue()
                    ? previous.Base64Decode().ColonDelimitedCursorToDictionary()
                    : next.Base64Decode().ColonDelimitedCursorToDictionary();

                direction = CursorProperties.TryGetCursorProperty<SortDirectionType>(nameof(direction));
                limit = CursorProperties.TryGetCursorProperty<uint>(nameof(limit));

                NextEncoded = CursorProperties.TryGetCursorProperty<string>(nameof(next));
                PreviousEncoded = CursorProperties.TryGetCursorProperty<string>(nameof(previous));
            }

            if (limit == 0 || limit > MaximumLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), $"Limit must be between 0 and {MaximumLimit}.");
            }

            if (!direction.IsValid())
            {
                throw new ArgumentException("Supplied sort direction must be ASC or DESC.");
            }

            Direction = direction;
            Limit = limit;
        }

        /// <summary>
        /// The next cursor (page) in a request. Would be a Base64 string if exists.
        /// </summary>
        private string NextEncoded { get; }

        /// <summary>
        /// Shorthand check for if the request included a next page to route too.
        /// </summary>
        public bool PagingForward => NextEncoded.HasValue();

        /// <summary>
        /// The decoded Base64 next cursor value as a string.
        /// </summary>
        protected string NextDecoded => NextEncoded.Base64Decode();

        /// <summary>
        /// The previous cursor (page) in a request. Would be a Base64 string if exists.
        /// </summary>
        private string PreviousEncoded { get; }

        /// <summary>
        /// Shorthand check for if the request included a previous page to route too.
        /// </summary>
        public bool PagingBackward => PreviousEncoded.HasValue();

        /// <summary>
        /// The decoded Base64 previous cursor value as a string.
        /// </summary>
        protected string PreviousDecoded => PreviousEncoded.Base64Decode();

        /// <summary>
        /// Shorthand check for if the request is not paging, but requesting the first page.
        /// </summary>
        protected bool IsNewRequest => !NextEncoded.HasValue() && !PreviousEncoded.HasValue();

        /// <summary>
        /// The page size of the request.
        /// </summary>
        public uint Limit { get; }

        /// <summary>
        /// The sort direction of the request.
        /// </summary>
        public SortDirectionType Direction { get; }

        /// <summary>
        /// The maximum page size of the request.
        /// </summary>
        private uint MaximumLimit { get; }

        /// <summary>
        /// A dictionary of keys and values of the passed in Base64 Encoded previous or next cursor.
        /// </summary>
        protected IDictionary<string, List<string>> CursorProperties { get; }
    }
}
