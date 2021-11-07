using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;

namespace Opdex.Platform.WebApi.Models.Requests
{
    /// <summary>
    /// Filter parameters for paginated queries.
    /// </summary>
    public abstract class FilterParameters<TCursor> where TCursor : Cursor
    {
        private TCursor _builtCursor;

        /// <summary>
        /// The order direction of the results, either "ASC" or "DESC".
        /// </summary>
        public SortDirectionType Direction { get; set; }

        /// <summary>
        /// Number of results to return per page.
        /// </summary>
        public uint Limit { get; set; }

        /// <summary>
        /// The cursor when paging.
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// Builds a cursor filter used for querying.
        /// </summary>
        public TCursor BuildCursor()
        {
            // returns lazily to avoid parsing multiple times
            if (_builtCursor is null) _builtCursor = InternalBuildCursor();
            return _builtCursor;
        }

        /// <summary>
        /// Indicates whether the filter parameters are valid.
        /// </summary>
        public bool ValidateWellFormed() => !(BuildCursor() is null);

        protected abstract TCursor InternalBuildCursor();
    }
}
