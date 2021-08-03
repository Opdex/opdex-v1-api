using System;
using System.Text;

namespace Opdex.Platform.Common.Extensions
{
    public static class Base64Extensions
    {
        /// <summary>
        /// Base64 envode a string
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

        public static bool TryBase64Decode(this string base64EncodedData, out string plainText)
        {
            plainText = "";

            Span<byte> decodedBytes = stackalloc byte[3 * base64EncodedData.Length / 4];

            var canDecode = Convert.TryFromBase64String(base64EncodedData, decodedBytes, out var bytesWritten);
            if (!canDecode) return false;

            plainText = Encoding.UTF8.GetString(decodedBytes.Slice(0, bytesWritten));
            return true;
        }
    }
}