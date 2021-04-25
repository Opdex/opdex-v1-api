using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens
{
    public class TokenSnapshotEntity : SnapshotEntity
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public decimal Price { get; set; }
    }
}