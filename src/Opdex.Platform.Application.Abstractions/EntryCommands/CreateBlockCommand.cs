using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands
{
    public class CreateBlockCommand : IRequest<bool>
    {
        public CreateBlockCommand(ulong height, string hash, string time, string medianTime)
        {
            var dateTime = time.FromUnixTimeSeconds();
            var dateMedianTime = medianTime.FromUnixTimeSeconds();
            
            if (height < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            if (!hash.HasValue())
            {
                throw new ArgumentNullException(nameof(hash));
            }

            if (dateTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(dateTime));
            }
            
            if (dateMedianTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(dateMedianTime));
            }

            Height = height;
            Hash = hash;
            Time = dateTime;
            MedianTime = dateMedianTime;
        }

        public ulong Height { get; }
        public string Hash { get; }
        public DateTime Time { get; }
        public DateTime MedianTime { get; }
    }
}