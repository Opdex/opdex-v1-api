using System;

namespace Opdex.Platform.WebApi.Models.Responses.Blocks
{
    public class BlockResponseModel
    {
        public string Hash { get; set; }
        public ulong Height { get; set; }
        public DateTime Time { get; set; }
        public DateTime MedianTime { get; set; }
    }
}
