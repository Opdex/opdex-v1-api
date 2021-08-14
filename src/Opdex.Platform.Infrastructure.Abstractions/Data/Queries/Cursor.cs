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
    public abstract class Cursor<TPointer> where TPointer : IEquatable<TPointer>
    {
        public Cursor(SortDirectionType sortDirection, uint limit, PagingDirection pagingDirection, TPointer pointer,
                      uint defaultLimit = 10, uint maxLimit = 50, SortDirectionType defaultSortDirection = SortDirectionType.DESC)
        {
            if (defaultLimit == 0) throw new ArgumentOutOfRangeException("Default limit must be greater than 0.");
            if (maxLimit == 0) throw new ArgumentOutOfRangeException("Max limit must be greater than 0.");
            if (defaultLimit > maxLimit) throw new ArgumentOutOfRangeException("Default limit cannot be greater than max limit.");
            if (!defaultSortDirection.IsValid()) throw new ArgumentOutOfRangeException("Invalid default sort direction.");

            if (sortDirection == default) SortDirection = defaultSortDirection;
            else SortDirection = sortDirection.IsValid() ? sortDirection : throw new ArgumentOutOfRangeException("Invalid sort direction.");

            if (limit == default) Limit = defaultLimit;
            else Limit = limit <= maxLimit ? limit : throw new ArgumentOutOfRangeException(nameof(limit), $"Limit cannot be greater than {maxLimit}.");

            PagingDirection = pagingDirection.IsValid() ? pagingDirection : throw new ArgumentOutOfRangeException(nameof(pagingDirection), "Invalid paging direction.");

            Pointer = ValidatePointer(pointer) ? pointer : throw new ArgumentException("Invalid cursor pointer.", nameof(pointer));
        }

        /// <summary>
        /// Order in which to sort items in the page
        /// </summary>
        public SortDirectionType SortDirection { get; }

        /// <summary>
        /// Number of results to return in the page
        /// </summary>
        public uint Limit { get; }

        /// <summary>
        /// Current direction of pagination
        /// </summary>
        public PagingDirection PagingDirection { get; }

        /// <summary>
        /// Points to a specific location in the data source
        /// </summary>
        public TPointer Pointer { get; }

        /// <summary>
        /// Whether this is the first request
        /// </summary>
        public virtual bool IsFirstRequest => PagingDirection == PagingDirection.Forward && Pointer.Equals(default);

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
        protected virtual bool ValidatePointer(TPointer pointer) => PagingDirection == PagingDirection.Forward || !pointer.Equals(default);

        /// <summary>
        /// Converts a stringified cursor into a dictionary
        /// </summary>
        /// <param name="raw">Stringified cursor</param>
        /// <returns>Dictionary for looking up cursor values</returns>
        protected static IDictionary<string, IEnumerable<string>> ToDictionary(string raw)
        {
            return raw.Split(';')
                      .Select(part => part.Split(':'))
                      .Where(part => part.Length == 2)
                      .Select(array => new { Key = array[0], Value = array[1] })
                      .GroupBy(part => part.Key, part => part.Value)
                      .ToDictionary(sp => sp.Key, sp => sp.ToList().AsEnumerable());
        }

        /// <summary>
        /// TryGet a single value from the cursor dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary containing to use.</param>
        /// <param name="key">The key to lookup the value decoded value of.</param>
        /// <typeparam name="TK">Generic used to cast any found results too.</typeparam>
        /// <returns>A single string record or null.</returns>
        protected static bool TryGetCursorProperty<TK>(IDictionary<string, IEnumerable<string>> dictionary, string key, out TK value)
        {
            value = default;

            // Return single record
            if (!TryGetCursorProperties<TK>(dictionary, key, out var values)) return false;
            if (values.Count != 1) return false;

            value = values[0];
            return true;
        }

        /// <summary>
        /// TryGet a list of generic values from a cursor dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary containing to use.</param>
        /// <param name="key">The key to lookup the value decoded value of.</param>
        /// <param name="values">The values that get retrieved</param>
        /// <typeparam name="TK">Generic used to cast any found results too.</typeparam>
        /// <returns>True if cursor properties were retrieved, otherwise false.</returns>
        protected static bool TryGetCursorProperties<TK>(IDictionary<string, IEnumerable<string>> dictionary, string key, out IReadOnlyList<TK> values)
        {
            values = new List<TK>().AsReadOnly();

            if (dictionary == null || !key.HasValue()) return false;

            // Get results return if none found
            if (!dictionary.TryGetValue(key, out var results)) return false;

            // If it's not an emum type, convert and return
            if (!typeof(TK).IsEnum)
            {
                try
                {
                    values = results.Select(result => (TK)Convert.ChangeType(result, typeof(TK))).ToList().AsReadOnly();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            // Assert all types that are enum, are valid values
            if (results.Any(result => !typeof(TK).IsEnumDefined(result))) return false;

            // Return list of TK enum values
            try
            {
                values = results.Select(result => (TK)Enum.Parse(typeof(TK), result)).ToList().AsReadOnly();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
