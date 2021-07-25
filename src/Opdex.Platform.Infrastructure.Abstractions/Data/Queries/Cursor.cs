using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries
{
    /// <summary>
    /// Can be implemented with queries to a data source, so that data can be retrieved in a paginated manor
    /// </summary>
    /// <typeparam name="TPointer">Pointer on which to mark location in the data source</typeparam>
    public abstract class Cursor<TPointer>
    {
        public Cursor(SortDirectionType orderBy, uint limit, PagingDirection pagingDirection, TPointer pointer)
        {
            OrderBy = orderBy.IsValid() ? orderBy : throw new ArgumentOutOfRangeException(nameof(orderBy), "Invalid sort direction.");
            Limit = limit > 0 ? limit : throw new ArgumentOutOfRangeException(nameof(limit), $"Limit must be greater than zero.");
            PagingDirection = pagingDirection.IsValid() ? pagingDirection : throw new ArgumentOutOfRangeException(nameof(pagingDirection), "Invalid paging direction.");
            Pointer = ValidatePointer(pointer) ? pointer : throw new ArgumentException(nameof(pointer), "Invalid cursor pointer.");
        }

        /// <summary>
        /// Order in which to sort items in the page
        /// </summary>
        public SortDirectionType OrderBy { get; }

        /// <summary>
        /// Number of results to return in the page
        /// </summary>
        public uint Limit { get; }

        /// <summary>
        /// Current direction of pagination
        /// </summary>
        public PagingDirection PagingDirection { get; }

        /// <summary>
        /// Whether this is the first requests page, in other words the pointer has a default value
        /// </summary>
        public abstract bool IsFirstRequest { get; }

        /// <summary>
        /// Points to a specific location in the data source
        /// </summary>
        public TPointer Pointer { get; }


        /// <summary>
        /// Creates a new cursor pointing to an adjacent page of results
        /// </summary>
        /// <param name="direction">Direction of page release to this</param>
        /// <param name="pointer">Next or previous pointer</param>
        /// <returns>Another cursor relative to this cursor</returns>
        public abstract Cursor<TPointer> Turn(PagingDirection direction, TPointer pointer);

        /// <inheritdoc />
        public abstract override string ToString();

        /// <summary>
        /// Checks that a provided pointer value is valid
        /// </summary>
        /// <returns>True if valid, otherwise false</returns>
        protected virtual bool ValidatePointer(TPointer pointer) => true;

        /// <summary>
        /// TryGet a single value from the cursor dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary containing to use.</param>
        /// <param name="key">The key to lookup the value decoded value of.</param>
        /// <typeparam name="TK">Generic used to cast any found results too.</typeparam>
        /// <returns>A single string record or null.</returns>
        protected static TK TryGetCursorProperty<TK>(IDictionary<string, IEnumerable<string>> dictionary, string key)
        {
            // Return single record
            return TryGetCursorProperties<TK>(dictionary, key).SingleOrDefault();
        }

        /// <summary>
        /// TryGet a list of generic values from a cursor dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary containing to use.</param>
        /// <param name="key">The key to lookup the value decoded value of.</param>
        /// <typeparam name="TK">Generic used to cast any found results too.</typeparam>
        /// <returns>List of type TK of found values.</returns>
        protected static IEnumerable<TK> TryGetCursorProperties<TK>(IDictionary<string, IEnumerable<string>> dictionary, string key)
        {
            if (dictionary == null || !key.HasValue())
            {
                throw new ArgumentException("The dictionary and a key must be provided.");
            }

            // Get results return empty list if none found
            var success = dictionary.TryGetValue(key, out var results);
            if (!success)
            {
                return Enumerable.Empty<TK>();
            }

            // If it's not an emum type, convert and return
            if (!typeof(TK).IsEnum)
            {
                return results.Select(result => (TK)Convert.ChangeType(result, typeof(TK)));
            }

            // Assert all types that are enum, are valid values
            if (results.Any(result => !typeof(TK).IsEnumDefined(result)))
            {
                throw new ArgumentOutOfRangeException(nameof(key), key, "Invalid enum type.");
            }

            // Return list of TK enum values
            return results.Select(result => (TK)Enum.Parse(typeof(TK), result));
        }
    }
}
