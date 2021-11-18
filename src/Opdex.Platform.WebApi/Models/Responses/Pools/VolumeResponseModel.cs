using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class VolumeResponseModel
    {
        public FixedDecimal Crs { get; set; }
        
        public FixedDecimal Src { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Usd { get; set; }
    }
}
