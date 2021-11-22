using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Blocks
{
    public class BlockResponseModel
    {
        public string Hash { get; set; }

        [Range(1, double.MaxValue)]
        public ulong Height { get; set; }

        public DateTime Time { get; set; }
        
        public DateTime MedianTime { get; set; }
    }
}
