using Microsoft.AspNetCore.Mvc;
using NJsonSchema.Annotations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System.ComponentModel.DataAnnotations;

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
        /// <example>DESC</example>
        public SortDirectionType Direction { get; set; }

        /// <summary>
        /// Number of results to return per page.
        /// </summary>
        /// <example>10</example>
        // virtual is used to be able to override the attribute
        [Range(1, Cursor.DefaultMaxLimit)]
        public virtual uint Limit { get; set; }

        /// <summary>
        /// The cursor when paging.
        /// </summary>
        /// <example>ZGlyZWN0aW9uOkRFU0M7bGltaXQ6MTA7cGFnaW5nOkZvcndhcmQ7b3JkZXJCeTpEZWZhdWx0O3BvaW50ZXI6S0N3Z05Taz07</example>
        [NotNull]
        [FromQuery(Name = "Cursor")]
        public string EncodedCursor { get; set; }

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
