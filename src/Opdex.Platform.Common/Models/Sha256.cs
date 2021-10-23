using Opdex.Platform.Common.Models.UInt;
using System;
using System.Globalization;
using System.Numerics;

namespace Opdex.Platform.Common.Models
{
    /// <summary>
    /// A Sha256 hash value.
    /// </summary>
    public readonly struct Sha256 : IComparable, IComparable<Sha256>, IEquatable<Sha256>
    {
        private const int HexEncodedLength = 64;

        /// <summary>
        /// Creates a Sha256 hash.
        /// </summary>
        /// <param name="value">The raw hash value.</param>
        public Sha256(UInt256 value)
        {
            Value = value;
        }

        /// <summary>
        /// Raw hash value.
        /// </summary>
        private UInt256 Value { get; }

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            if (obj is Sha256 other) return CompareTo(other);
            throw new ArgumentException("Other value must be of type Sha256.", nameof(obj));
        }

        /// <inheritdoc />
        public int CompareTo(Sha256 other) => Value.CompareTo(other.Value);

        /// <inheritdoc />
        public bool Equals(Sha256 other) => Value == other.Value;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Sha256 other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString()
        {
            // TODO: think about cleaning this up by amending UIntBase/256 to support formatting
            var hash = ((BigInteger)Value).ToString("x64", CultureInfo.InvariantCulture);
            if (hash.Length == 65) hash = hash[1..]; // BigInteger can prepend a 0 to indicate the +/- sign, we remove this
            return hash;
        }

        public static bool operator ==(Sha256 a, Sha256 b) => a.Equals(b);

        public static bool operator !=(Sha256 a, Sha256 b) => !a.Equals(b);

        /// <summary>
        /// Converts the hexadecimal string representation of a SHA256 hash to its <see cref="Sha256" /> equivalent.
        /// </summary>
        /// <param name="value">The hexadecimal encoded hash.</param>
        /// <returns>The Sha256 hash representation of the <see cref="value" /> parameter.</returns>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        public static Sha256 Parse(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (value.Length != HexEncodedLength) throw new ArgumentException($"Value must be a {HexEncodedLength} character hex string.", nameof(value));
            return new Sha256(UInt256.Parse($"0x{value}"));
        }

        /// <summary>
        /// Tries to convert the hexadecimal representation of a SHA256 hash to its <see cref="Sha256" /> equivalent, and returns a value that indicates
        /// whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The hexadecimal string representation of a number.</param>
        /// <param name="result">
        /// When this method returns true, contains the <see cref="Sha256" /> equivalent to the hash that is contained in value,
        /// or a zero-hash if the conversion failed.
        /// </param>
        /// <returns><see cref="true" /> if <see cref="value" /> was parsed successfully; otherwise, <see cref="false" />.</returns>
        public static bool TryParse(string value, out Sha256 hash)
        {
            hash = default;
            if (value is null || value.Length != HexEncodedLength) return false;

            // first digit identifies + value
            if (!BigInteger.TryParse($"0{value}", NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result)) return false;

            hash = new Sha256((UInt256)result);
            return true;
        }
    }
}
