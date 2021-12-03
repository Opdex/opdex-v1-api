using System;
using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Blocks;

public class MakeBlockCommand : IRequest<bool>
{
    public MakeBlockCommand(ulong height, Sha256 hash, DateTime time, DateTime medianTime)
    {
        Height = height;
        Hash = hash;
        Time = time;
        MedianTime = medianTime;
    }

    public ulong Height { get; }
    public Sha256 Hash { get; }
    public DateTime Time { get; }
    public DateTime MedianTime { get; }
}