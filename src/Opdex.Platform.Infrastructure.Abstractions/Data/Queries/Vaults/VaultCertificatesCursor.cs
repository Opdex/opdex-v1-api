using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults
{
    public class VaultCertificatesCursor
    {
        private const uint MaxLimit = 100;

        public VaultCertificatesCursor(string holder, SortDirectionType direction, uint limit, long id, PagingDirection pagingDirection)
        {
            Holder = holder;
            Direction = direction.IsValid() ? direction : throw new ArgumentOutOfRangeException(nameof(direction), "Invalid sort direction.");
            Limit = limit > 0 && limit <= MaxLimit ? limit : throw new ArgumentOutOfRangeException(nameof(limit), $"Limit must be between 1 and {MaxLimit}.");
            Id = id >= 0 ? id : throw new ArgumentOutOfRangeException(nameof(id), "Id cannot be less than zero.");
            PagingDirection = pagingDirection.IsValid() ? pagingDirection : throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
        }

        public string Holder { get; }
        public SortDirectionType Direction { get; }
        public uint Limit { get; }
        public long Id { get; }
        public PagingDirection PagingDirection { get; }

        public bool IsNewRequest => Id == 0;

        public VaultCertificatesCursor Turn(long id)
        {
            if (id == Id) throw new ArgumentOutOfRangeException(nameof(id), "Cannot paginate with an identical id.");

            PagingDirection requestedPagingDirection = PagingDirection.Forward;

            if (!IsNewRequest)
            {
                if (PagingDirection == PagingDirection.Forward && Direction == SortDirectionType.ASC)
                {
                    requestedPagingDirection = id >= Id + Limit ? PagingDirection.Forward : PagingDirection.Backward;
                }
                else if (PagingDirection == PagingDirection.Backward && Direction == SortDirectionType.ASC)
                {
                    requestedPagingDirection = id > Id - Limit ? PagingDirection.Forward : PagingDirection.Backward;
                }
                else if (PagingDirection == PagingDirection.Forward && Direction == SortDirectionType.DESC)
                {
                    requestedPagingDirection = id <= Id - Limit ? PagingDirection.Forward : PagingDirection.Backward;
                }
                else if (PagingDirection == PagingDirection.Backward && Direction == SortDirectionType.DESC)
                {
                    requestedPagingDirection = id < Id + Limit ? PagingDirection.Forward : PagingDirection.Backward;
                }
                else
                {
                    throw new NotSupportedException("Unrecognised paging and sort direction combination");
                }
            }

            return new VaultCertificatesCursor(Holder, Direction, Limit, id, requestedPagingDirection);
        }

        public override string ToString()
        {
            var pointerBytes = Encoding.UTF8.GetBytes(Id.ToString());
            var pointer = Convert.ToBase64String(pointerBytes);
            return $"holder:{Holder};direction:{Direction};limit:{Limit};paging:{PagingDirection};pointer:{pointer};";
        }

        public static bool TryParse(string raw, out VaultCertificatesCursor cursor)
        {
            cursor = null;

            var values = raw.Split(';')
                            .Select(part => part.Split(':'))
                            .Where(part => part.Length == 2)
                            .Select(array => new { Key = array[0], Value = array[1] })
                            .GroupBy(part => part.Key, part => part.Value)
                            .ToDictionary(sp => sp.Key, sp => sp.ToList().AsEnumerable());

            var holder = TryGetCursorProperty<string>(values, "holder");

            var direction = TryGetCursorProperty<SortDirectionType>(values, "direction");
            if (!direction.IsValid()) return false;

            var limit = TryGetCursorProperty<uint>(values, "limit");

            var pointerEncoded = TryGetCursorProperty<string>(values, "pointer");

            if (!pointerEncoded.HasValue()) return false;

            TryDecodePointer(pointerEncoded, out var pointer);
            if (pointer == 0) return false;

            var paging = TryGetCursorProperty<PagingDirection>(values, "paging");

            cursor = new VaultCertificatesCursor(holder, direction, limit, pointer, paging);

            return true;
        }

        private static bool TryDecodePointer(string encoded, out long pointer)
        {
            pointer = 0;

            Span<byte> decodedBytes = stackalloc byte[3 * encoded.Length / 4];

            var canDecode = Convert.TryFromBase64String(encoded, decodedBytes, out var bytesWritten);
            if (!canDecode) return false;

            var decoded = Encoding.UTF8.GetString(decodedBytes.Slice(0, bytesWritten));
            if (!long.TryParse(decoded, out var result))
            {
                return false;
            }

            pointer = result;
            return true;
        }

        /// <summary>
        /// TryGet a single value from the cursor dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary containing to use.</param>
        /// <param name="key">The key to lookup the value decoded value of.</param>
        /// <typeparam name="TK">Generic used to cast any found results too.</typeparam>
        /// <returns>A single string record or null.</returns>
        private static TK TryGetCursorProperty<TK>(IDictionary<string, IEnumerable<string>> dictionary, string key)
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
        private static IEnumerable<TK> TryGetCursorProperties<TK>(IDictionary<string, IEnumerable<string>> dictionary, string key)
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
