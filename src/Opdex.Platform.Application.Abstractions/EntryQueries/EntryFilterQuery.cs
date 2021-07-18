using MediatR;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Application.Abstractions.EntryQueries
{
    public abstract class EntryFilterQuery<T> : IRequest<T>
    {
        protected EntryFilterQuery(string direction, uint limit, uint maxLimit, string next, string previous)
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
                var decoded = previous.HasValue() ? previous.Base64Decode() : next.Base64Decode();

                DecodedCursorDictionary = decoded
                    .Split(';')
                    .Select(part => part.Split(':'))
                    .Where(part => part.Length == 2)
                    .Select(array => new {Key = array[0], Value = array[1]})
                    .GroupBy(part => part.Key, part => part.Value)
                    .ToDictionary(sp => sp.Key, sp => sp.ToList());

                var cursorDirection = TryGetCursorDictionarySingle(nameof(direction));
                var cursorLimit = TryGetCursorDictionarySingle(nameof(limit));

                Next = TryGetCursorDictionarySingle(nameof(next));
                Previous = TryGetCursorDictionarySingle(nameof(previous));

                direction = cursorDirection;
                limit = uint.Parse(cursorLimit);
            }

            ValidateBaseParameters(direction, limit);
        }

        /// <summary>
        /// The next cursor (page) in a request. Would be a Base64 string if exists.
        /// </summary>
        private string Next { get; }

        /// <summary>
        /// Shorthand check for if the request included a next page to route too.
        /// </summary>
        public bool PagingForward => Next.HasValue();

        /// <summary>
        /// The decoded Base64 next cursor value as a string.
        /// </summary>
        protected string NextDecoded => Next.Base64Decode();

        /// <summary>
        /// The previous cursor (page) in a request. Would be a Base64 string if exists.
        /// </summary>
        private string Previous { get; }

        /// <summary>
        /// Shorthand check for if the request included a previous page to route too.
        /// </summary>
        public bool PagingBackward => Previous.HasValue();

        /// <summary>
        /// The decoded Base64 previous cursor value as a string.
        /// </summary>
        protected string PreviousDecoded => Previous.Base64Decode();

        /// <summary>
        /// Shorthand check for if the request is not paging, but requesting the first page.
        /// </summary>
        protected bool IsNewQuery => !Next.HasValue() && !Previous.HasValue();

        /// <summary>
        /// The page size of the request.
        /// </summary>
        public uint Limit { get; private set; }

        /// <summary>
        /// The sort direction of the request.
        /// </summary>
        public string Direction { get; private set; }

        /// <summary>
        /// The maximum page size of the request.
        /// </summary>
        private uint MaximumLimit { get; }

        /// <summary>
        /// A dictionary of keys and values of the passed in Base64 Encoded previous or next cursor.
        /// </summary>
        private Dictionary<string, List<string>> DecodedCursorDictionary { get; }

        /// <summary>
        /// TryGet a single string value from the cursor dictionary based on the provided key.
        /// </summary>
        /// <remarks>
        /// The cursor dictionary stores all values as a list of strings, this method explicitly selects a Single
        /// record if the provided key has a value or returns null.
        /// </remarks>
        /// <param name="key">The key to lookup the value decoded value of.</param>
        /// <returns>A single string record or null.</returns>
        protected string TryGetCursorDictionarySingle(string key)
        {
            return DecodedCursorDictionary.TryGetValue(key, out var result) ? result.Single() : null;
        }

        /// <summary>
        /// TryGet a list of generic values from the cursor dictionary based on the provided key.
        /// </summary>
        /// <param name="key">The key to lookup the value decoded value of.</param>
        /// <typeparam name="TK">Generic used to cast any found results too.</typeparam>
        /// <returns>List of type TK of found values.</returns>
        protected List<TK> TryGetCursorDictionaryList<TK>(string key)
        {
            // Get results return empty list if none found
            var success = DecodedCursorDictionary.TryGetValue(key, out var results);
            if (!success || results.Count < 1)
            {
                return new List<TK>();
            }

            // If it's not an emum type, cast and return
            if (!typeof(TK).IsEnum)
            {
                return results.Cast<TK>().ToList();
            }

            // Assert all types that are enum, are valid values
            if (results.Any(result => !typeof(TK).IsEnumDefined(result)))
            {
                throw new ArgumentOutOfRangeException(nameof(key), key, "Invalid enum type.");
            }

            // Return list of TK enum values
            return results.Select(result => (TK)Enum.Parse(typeof(TK), result)).ToList();
        }

        private void ValidateBaseParameters(string direction, uint limit)
        {
            if (limit == 0 || limit > MaximumLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), $"Max limit is {MaximumLimit}.");
            }

            if (direction.HasValue() && (!direction.EqualsIgnoreCase("ASC") && !direction.EqualsIgnoreCase("DESC")))
            {
                throw new ArgumentException("Supplied sort direction must be ASC or DESC.");
            }

            Direction = direction ?? "DESC";
            Limit = limit;
        }
    }
}
