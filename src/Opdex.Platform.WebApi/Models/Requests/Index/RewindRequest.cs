using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Index
{
    /// <summary>
    /// Request to rewind back to a specific block in time.
    /// </summary>
    public class RewindRequest
    {
        /// <summary>
        /// The block number to rewind too.
        /// </summary>
        /// <example>500000</example>
        [Required]
        [Range(1, double.MaxValue)]
        public ulong Block { get; set; }
    }
}
