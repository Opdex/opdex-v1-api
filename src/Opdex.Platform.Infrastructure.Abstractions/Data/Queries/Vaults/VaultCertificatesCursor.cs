using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults
{
    public class VaultCertificatesCursor : Cursor<long>
    {
        public const uint MaxLimit = 50;

        public VaultCertificatesCursor(string holder, SortDirectionType orderBy, uint limit, PagingDirection pagingDirection, long pointer)
            : base(orderBy, limit, pagingDirection, pointer)
        {
            if (limit > MaxLimit) throw new ArgumentOutOfRangeException(nameof(limit), $"Limit exceeds maximum limit of {MaxLimit}.");
            Holder = holder;
        }

        /// <summary>
        /// Filter for a particular certificate holder
        /// </summary>
        public string Holder { get; }

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

            return new VaultCertificatesCursor(Holder, OrderBy, Limit, direction, pointer);
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

            if (raw is null) return false;

            var values = ToDictionary(raw);

            TryGetCursorProperty<string>(values, "holder", out var holder);

            if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

            if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

            if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

            if (!pointerEncoded.HasValue()) return false;

            if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

            if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

            try
            {
                cursor = new VaultCertificatesCursor(holder, direction, limit, paging, pointer);
            }
            catch (Exception)
            {
                return false;
            }

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
