using System;
using MediatR;

namespace Opdex.Indexer.Application.Abstractions.Commands.Blocks
{
    public class MakeBlockCommand : IRequest<bool>
    {
        public MakeBlockCommand(ulong height, string hash, DateTime time, DateTime medianTime)
        {
            Height = height;
            Hash = hash;
            Time = time;
            MedianTime = medianTime;
        }
        
        public ulong Height { get; }
        public string Hash { get; }
        public DateTime Time { get; }
        public DateTime MedianTime { get; }
    }
}