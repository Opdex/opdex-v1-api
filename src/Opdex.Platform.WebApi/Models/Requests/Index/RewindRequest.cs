using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Index
{
    /// <summary>
    /// Public request object used to rewind back to a specific block in time.
    /// </summary>
    public class RewindRequest
    {
        /// <summary>
        /// The block number to rewind too.
        /// </summary>
        [Required]
        [Range(1, double.MaxValue)]
        public ulong Block { get; set; }
    }
}
