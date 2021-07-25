using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Vaults
{
    public class GetVaultCertificatesWithFilterQuery : EntryFilterQuery<CertificatesDto>
    {
        private const uint MaxLimit = 100;

        public GetVaultCertificatesWithFilterQuery(string vault, string holder, SortDirectionType direction, uint limit, string next, string previous) : base(direction, limit, MaxLimit, next, previous)
        {
            Vault = vault.HasValue() ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be set.");

            // Decode the Previous cursor if it's provided and validate the value
            var parsedPrevious = long.TryParse(PreviousDecoded, out var previousParsed);
            if (PreviousDecoded.HasValue() && !parsedPrevious)
            {
                throw new ArgumentOutOfRangeException(nameof(previous), "Invalid previous cursor value.");
            }

            // Decode the Next cursor if it's provided and validate the value
            var parsedNext = long.TryParse(NextDecoded, out var nextParsed);
            if (NextDecoded.HasValue() && !parsedNext)
            {
                throw new ArgumentOutOfRangeException(nameof(next), "Invalid next cursor value.");
            }

            Previous = previousParsed;
            Next = nextParsed;
            Holder = IsNewRequest ? holder : CursorProperties.TryGetCursorProperty<string>(nameof(holder));
        }

        public string Vault { get; }
        public string Holder { get; }
        public long Next { get; }
        public long Previous { get; }
    }
}
