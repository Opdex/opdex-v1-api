using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.Models.Blocks;

public class BlockDto
{
    public ulong Height { get; set; }
    public Sha256 Hash { get; set; }
    public DateTime Time { get; set; }
    public DateTime MedianTime { get; set; }
}
