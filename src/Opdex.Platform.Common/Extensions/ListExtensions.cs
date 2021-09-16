using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Common.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Split an IEnumerable into multiple smaller chunks.
        /// </summary>
        /// <param name="items">The items to split up.</param>
        /// <param name="itemsPerChunk">The number of items per chunked list.</param>
        /// <typeparam name="T">The type of IEnumerable being chunked.</typeparam>
        /// <returns>2 dimensional array of chunked lists.</returns>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> items, int itemsPerChunk)
        {
            return items
                .Select((item, i) => new { Value = item, Index = i })
                .GroupBy(x => x.Index / itemsPerChunk)
                .Select(g => g.Select(x => x.Value));
        }
    }
}
