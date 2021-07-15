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
            MaximumLimit = maxLimit;

            if (next.HasValue() && previous.HasValue())
            {
                throw new ArgumentException("Next and previous cannot both have values.");
            }

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

                ValidateBaseParameters(cursorDirection, uint.Parse(cursorLimit));
            }
            else
            {
                ValidateBaseParameters(direction, limit);
            }
        }

        public bool IsNewRequest => !Previous.HasValue() && !Next.HasValue();
        public string Next { get; }
        public string NextDecoded => Next.Base64Decode();
        public string Previous { get; }
        public string PreviousDecoded => Previous.Base64Decode();
        public uint Limit { get; private set; }
        public string Direction { get; private set; }
        public uint MaximumLimit { get; }
        public Dictionary<string, List<string>> DecodedCursorDictionary { get; }

        protected string TryGetCursorDictionarySingle(string key)
        {
            return DecodedCursorDictionary.TryGetValue(key, out var result) ? result.Single() : null;
        }

        protected List<TK> TryGetCursorDictionaryList<TK>(string key)
        {
            return DecodedCursorDictionary.TryGetValue(key, out var result) ? result as List<TK> : new List<TK>();
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
