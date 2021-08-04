using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions
{
    public class TransactionsCursor : Cursor<long>
    {
        public TransactionsCursor(string wallet, IEnumerable<TransactionEventType> eventTypes,
                                  IEnumerable<string> contracts, SortDirectionType sortDirection, uint limit,
                                  PagingDirection pagingDirection, long pointer)
            : base(sortDirection, limit, pagingDirection, pointer)
        {
            Wallet = wallet;
            EventTypes = eventTypes ?? Enumerable.Empty<TransactionEventType>();
            Contracts = contracts ?? Enumerable.Empty<string>();
        }

        public string Wallet { get; }
        public IEnumerable<TransactionEventType> EventTypes { get; }
        public IEnumerable<string> Contracts { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var pointerBytes = Encoding.UTF8.GetBytes(Pointer.ToString());
            var encodedPointer = Convert.ToBase64String(pointerBytes);

            var sb = new StringBuilder();
            sb.AppendFormat("direction:{0};limit:{1};paging:{2};", SortDirection, Limit, PagingDirection);
            sb.AppendFormat("wallet:{0};", Wallet);
            foreach (var eventType in EventTypes) sb.AppendFormat("eventTypes:{0};", eventType);
            foreach (var contract in Contracts) sb.AppendFormat("contracts:{0};", contract);
            sb.AppendFormat("pointer:{0};", encodedPointer);
            return sb.ToString();
        }

        /// <inheritdoc />
        public override Cursor<long> Turn(PagingDirection direction, long pointer)
        {
            if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
            if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical id.");

            return new TransactionsCursor(Wallet, EventTypes, Contracts, SortDirection, Limit, direction, pointer);
        }

        /// <inheritdoc />
        protected override bool ValidatePointer(long pointer) => pointer >= 0;

        /// <summary>
        /// Parses a stringified version of the cursor
        /// </summary>
        /// <param name="raw">Stringified cursor</param>
        /// <param name="cursor">Parsed cursor</param>
        /// <returns>True if the value could be parsed, otherwise false</returns>
        public static bool TryParse(string raw, out TransactionsCursor cursor)
        {
            cursor = null;

            if (raw is null) return false;

            var values = ToDictionary(raw);

            TryGetCursorProperty<string>(values, "wallet", out var wallet);

            TryGetCursorProperties<TransactionEventType>(values, "eventTypes", out var eventTypes);

            TryGetCursorProperties<string>(values, "contracts", out var contracts);

            if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

            if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

            if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

            if (!pointerEncoded.HasValue()) return false;

            if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

            if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

            try
            {
                cursor = new TransactionsCursor(wallet, eventTypes, contracts, direction, limit, paging, pointer);
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
