using Opdex.Platform.Common.Extensions;
using System.Text;

namespace Opdex.Platform.Application.Abstractions.Models
{
    public class CursorDto
    {
        public string Next { get; set; }
        public string Previous { get; set; }

        public void BuildNextCursor(string baseCursor, string cursorValue)
        {
            Next = BuildCursor(baseCursor, nameof(Next).ToLowerInvariant(), cursorValue);
        }

        public void BuildPreviousCursor(string baseCursor, string cursorValue)
        {
            Previous = BuildCursor(baseCursor, nameof(Previous).ToLowerInvariant(), cursorValue);
        }

        private static string BuildCursor(string baseCursor, string key, string cursorValue)
        {
            return new StringBuilder(baseCursor)
                .Append($"{key.ToLowerInvariant()}:{cursorValue.Base64Encode()};")
                .ToString()
                .Base64Encode();
        }
    }
}
