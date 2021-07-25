using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;
using System.Linq;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults
{
    public class VaultCertificatesCursor : Cursor<long>
    {
        public const uint MaxLimit = 50;

        public VaultCertificatesCursor(string holder, SortDirectionType orderBy, uint limit, long id, PagingDirection pagingDirection)
            : base(orderBy, limit, pagingDirection, id)
        {
            if (limit > MaxLimit) throw new ArgumentOutOfRangeException(nameof(limit), $"Limit exceeds maximum limit of {MaxLimit}.");
            Holder = holder;
        }

        /// <summary>
        /// Filter for a particular certificate holder
        /// </summary>
        public string Holder { get; }

        /// <inheritdoc />
        public override bool IsFirstRequest => Pointer == 0;

        /// <inheritdoc />
        public override string ToString()
        {
            var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
            var encodedPointer = Convert.ToBase64String(pointerBytes);
            return $"holder:{Holder};direction:{OrderBy};limit:{Limit};paging:{PagingDirection};pointer:{encodedPointer};";
        }

        /// <inheritdoc />
        public override Cursor<long> Turn(PagingDirection direction, long pointer)
        {
            if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
            if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical id.");

            return new VaultCertificatesCursor(Holder, OrderBy, Limit, pointer, direction);
        }

        /// <inheritdoc />
        protected override bool ValidatePointer(long pointer) => pointer >= 0;

        /// <summary>
        /// Parses a stringified version of the cursor
        /// </summary>
        /// <param name="raw">Stringified cursor</param>
        /// <param name="cursor">Parsed cursor</param>
        /// <returns>True if the value could be parsed, otherwise false</returns>
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

            if (!Base64Extensions.TryBase64Decode(encoded, out var decoded) || !long.TryParse(decoded, out var result)) return false;

            pointer = result;
            return true;
        }
    }
}
