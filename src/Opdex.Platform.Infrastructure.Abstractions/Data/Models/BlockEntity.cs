using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models;

public class BlockEntity
{
    public ulong Height { get; set; }
    public Sha256 Hash { get; set; }
    public DateTime Time { get; set; }
    public DateTime MedianTime { get; set; }
}