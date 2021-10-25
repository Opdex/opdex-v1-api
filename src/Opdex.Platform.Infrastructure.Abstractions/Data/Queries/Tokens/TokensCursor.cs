using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens
{
    public class TokensCursor : Cursor<(string, ulong)>
    {
        public TokensCursor(string keyword, IEnumerable<Address> tokens, IEnumerable<TokenAttributeType> attributes, TokenOrderByType orderBy,
                            SortDirectionType sortDirection, uint limit, PagingDirection pagingDirection, (string, ulong) pointer )
            : base(sortDirection, limit, pagingDirection, pointer)
        {
            Keyword = keyword;
            OrderBy = orderBy;
            Tokens = tokens ?? Enumerable.Empty<Address>();
            Attributes = attributes ?? Enumerable.Empty<TokenAttributeType>();
        }

        /// <summary>
        /// Can be any string, used to filter by address, name or symbol.
        /// </summary>
        public string Keyword { get; }

        /// <summary>
        /// The indexed, available columns to order retrieved tokens by.
        /// </summary>
        public TokenOrderByType OrderBy { get; }

        /// <summary>
        /// List of specific tokens to fetch.
        /// </summary>
        public IEnumerable<Address> Tokens { get; }

        /// <summary>
        /// Token attributes to filter for.
        /// </summary>
        public IEnumerable<TokenAttributeType> Attributes { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
            var encodedPointer = Convert.ToBase64String(pointerBytes);

            var sb = new StringBuilder();
            sb.AppendFormat("direction:{0};limit:{1};paging:{2};", SortDirection, Limit, PagingDirection);
            foreach (var token in Tokens) sb.AppendFormat("tokens:{0};", token);
            foreach (var attr in Attributes) sb.AppendFormat("attributes:{0};", attr);
            sb.AppendFormat("keyword:{0};", Keyword);
            sb.AppendFormat("orderBy:{0};", OrderBy);
            sb.AppendFormat("pointer:{0};", encodedPointer);
            return sb.ToString();
        }

        /// <inheritdoc />
        public override Cursor<(string, ulong)> Turn(PagingDirection direction, (string, ulong) pointer)
        {
            if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
            if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical pointer.");

            return new TokensCursor(Keyword, Tokens, Attributes, OrderBy, SortDirection, Limit, direction, pointer);
        }

        /// <summary>
        /// Parses a stringified version of the cursor
        /// </summary>
        /// <param name="raw">Stringified cursor</param>
        /// <param name="cursor">Parsed cursor</param>
        /// <returns>True if the value could be parsed, otherwise false</returns>
        public static bool TryParse(string raw, out TokensCursor cursor)
        {
            cursor = null;

            if (raw is null) return false;

            var values = ToDictionary(raw);

            TryGetCursorProperty<string>(values, "keyword", out var keyword);

            TryGetCursorProperty<TokenOrderByType>(values, "orderBy", out var orderBy);

            TryGetCursorProperties<Address>(values, "tokens", out var tokens);

            TryGetCursorProperties<TokenAttributeType>(values, "attributes", out var attributes);

            if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

            if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

            if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

            if (!pointerEncoded.HasValue()) return false;

            if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

            if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

            try
            {
                cursor = new TokensCursor(keyword, tokens, attributes, orderBy, direction, limit, paging, pointer);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool TryDecodePointer(string encoded, out (string, ulong) pointer)
        {
            pointer = (string.Empty, 0);

            if (!encoded.TryBase64Decode(out var decoded)) return false;

            var tupleParts = decoded.Replace("(", "").Replace(")", "").Split(',');

            if (tupleParts.Length != 2 || !ulong.TryParse(tupleParts[1], out var ulongValue))
            {
                return false;
            }

            pointer = (tupleParts[0], ulongValue);

            return true;
        }
    }
}
