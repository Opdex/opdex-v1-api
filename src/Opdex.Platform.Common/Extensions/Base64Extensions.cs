using System;
using System.Linq;
using System.Text;

namespace Opdex.Platform.Common.Extensions
{
    public static class Base64Extensions
    {
        /// <summary>
        /// Encode a Base64 string
        /// </summary>
        /// <param name="plainText">The plain text to encode.</param>
        /// <returns>The Base64 string</returns>
        public static string Base64Encode(this string plainText)
        {
            if (!plainText.HasValue()) return plainText;

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Decode a Base64 string
        /// </summary>
        /// <param name="base64EncodedData">Base64 string to decode</param>
        /// <returns>The decoded string</returns>
        public static string Base64Decode(this string base64EncodedData)
        {
            if (!base64EncodedData.HasValue()) return base64EncodedData;

            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Attempts to decode base64 data into plain text. Supports padding.
        /// </summary>
        /// <param name="base64EncodedData">The base64 encoded data to decode.</param>
        /// <param name="plainText">The decoded data.</param>
        /// <returns>True if the input is valid base64 encoded data, otherwise false.</returns>
        public static bool TryBase64Decode(this string base64EncodedData, out string plainText)
        {
            plainText = "";
            if (string.IsNullOrEmpty(base64EncodedData) || base64EncodedData.All(character => character == '=')) return false;

            var base64EncodedDataWithoutPadding = base64EncodedData.TrimEnd('=');
            Span<byte> decodedBytes = stackalloc byte[3 * base64EncodedDataWithoutPadding.Length / 4];

            var canDecode = Convert.TryFromBase64String(base64EncodedData, decodedBytes, out var bytesWritten);
            if (!canDecode) return false;

            plainText = Encoding.UTF8.GetString(decodedBytes[..bytesWritten]);
            return true;
        }

        public static string UrlSafeBase64Encode(ReadOnlySpan<byte> value)
        {
            return Convert.ToBase64String(value, Base64FormattingOptions.None).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public static ReadOnlySpan<byte> UrlSafeBase64Decode(string value)
        {
            value = value.Replace('-', '+').Replace('_', '/');
            if (value.Length % 4 != 0) value = value.PadRight(value.Length + (4 - (value.Length % 4)), '=');
            return Convert.FromBase64String(value);
        }
    }
}
