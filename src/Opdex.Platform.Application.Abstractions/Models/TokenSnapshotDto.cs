using System;

namespace Opdex.Platform.Application.Abstractions.Models
{
    public class TokenSnapshotDto
    {
        public long Id { get; set; }
        public TokenDto Token { get; set; }
        public decimal Price { get; set; }
        public int SnapshotType { get; set; }
        public DateTime SnapshotStartDate { get; set; }
        public DateTime SnapshotEndDate { get; set; }
    }
}