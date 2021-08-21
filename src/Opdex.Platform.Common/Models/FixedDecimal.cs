using System;
using System.Numerics;

namespace Opdex.Platform.Common.Models
{
    /// <summary>Represents an arbitrarily large signed fixed-point number.</summary>
    /// <remarks>The number is stored with a maximum precision of 255.</remarks>
    public readonly struct FixedDecimal : IComparable, IComparable<FixedDecimal>, IEquatable<FixedDecimal>
    {
        public static readonly FixedDecimal Zero = default;

        private FixedDecimal(BigInteger value, byte precision)
        {
            Value = value;
            Precision = precision;
        }

        private BigInteger Value { get; }

        private byte Precision { get; }

        /// <inheritdoc />
        /// <exception cref="OutOfMemoryException">Thrown if either underlying value is too large.</exception>
        public int CompareTo(FixedDecimal other)
        {
            var factor = Precision - other.Precision;
            return factor >= 0 ? Value.CompareTo(Scale(other.Value, factor))
                               : Scale(Value, factor * -1).CompareTo(other.Value);
        }

        /// <inheritdoc />
        /// <exception cref="OutOfMemoryException">Thrown if either underlying value is too large.</exception>
        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            if (obj is FixedDecimal other) return CompareTo(other);
            throw new ArgumentException("Other value must be of type FixedDecimal.", nameof(obj));
        }

        /// <inheritdoc />
        /// <exception cref="OutOfMemoryException">Thrown if either underlying value is too large.</exception>
        public bool Equals(FixedDecimal other)
        {
            var factor = Precision - other.Precision;
            return factor >= 0 ? Value == Scale(other.Value, factor)
                               : Scale(Value, factor * -1) == other.Value;
        }

        /// <inheritdoc />
        /// <exception cref="OutOfMemoryException">Thrown if either underlying value is too large.</exception>
        public override bool Equals(object obj) => obj is FixedDecimal other && Equals(other);

        /// <inheritdoc />
        /// <exception cref="OutOfMemoryException">Thrown if the underlying value is too large.</exception>
        public override int GetHashCode() => Scale(Value, byte.MaxValue - Precision).GetHashCode();

        /// <inheritdoc />
        public override string ToString()
        {
            var stringified = Value.ToString();
            if (Precision == 0) return stringified;

            var padded = stringified.PadLeft(Precision, '0');
            var decimalIndex = padded.Length - Precision;
            var result = padded.Insert(decimalIndex, decimalIndex != 0 ? "." : "0."); // include leading 0
            return result;
        }

        /// <exception cref="OutOfMemoryException">Thrown if the underlying value is too large.</exception>
        private static BigInteger Scale(BigInteger value, int factor) => value * BigInteger.Pow(10, factor);

        public static bool operator ==(FixedDecimal a, FixedDecimal b) => a.Equals(b);
        public static bool operator !=(FixedDecimal a, FixedDecimal b) => !a.Equals(b);
        public static bool operator >(FixedDecimal a, FixedDecimal b) => a.CompareTo(b) > 0;
        public static bool operator <(FixedDecimal a, FixedDecimal b) => a.CompareTo(b) < 0;
        public static bool operator >=(FixedDecimal a, FixedDecimal b) => a.CompareTo(b) >= 0;
        public static bool operator <=(FixedDecimal a, FixedDecimal b) => a.CompareTo(b) <= 0;

        /// <summary>Adds two numbers.</summary>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="OutOfMemoryException">Thrown if the <see cref="FixedDecimal" /> gets too large.</exception>
        public static FixedDecimal operator +(FixedDecimal a, FixedDecimal b)
        {
            var factor = a.Precision - b.Precision;
            return factor >= 0 ? new FixedDecimal(a.Value + Scale(b.Value, factor), a.Precision)
                               : new FixedDecimal(Scale(a.Value, factor * -1) + b.Value, b.Precision);
        }

        /// <summary>Subtracts the right number from the left.</summary>
        /// <returns>The result of the subtraction.</returns>
        /// <exception cref="OutOfMemoryException">Thrown if either underlying value is too large.</exception>
        public static FixedDecimal operator -(FixedDecimal a, FixedDecimal b)
        {
            var factor = a.Precision - b.Precision;
            return factor >= 0 ? new FixedDecimal(a.Value - Scale(b.Value, factor), a.Precision)
                               : new FixedDecimal(Scale(a.Value, factor * -1) - b.Value, b.Precision);
        }

        /// <summary>Converts the string representation of a number to its <see cref="FixedDecimal" /> equivalent.</summary>
        /// <param name="value">The stringified decimal value</param>
        /// <returns>A value that is equivalent to the number specified in the <see cref="value" /> parameter.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        /// <exception cref="FormatException" />
        public static FixedDecimal Parse(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            var index = value.IndexOf('.');
            if (index == -1) return new FixedDecimal(BigInteger.Parse(value), 0);

            var precision = value.Length - (index + 1);
            if (precision > byte.MaxValue) throw new ArgumentException("Supplied value exceeds maximum precision.", nameof(value));
            return new FixedDecimal(BigInteger.Parse(value.Remove(index, 1)), (byte)precision);
        }

        /// <summary>
        /// Tries to convert the string representation of a number to its <see cref="FixedDecimal" /> equivalent, and returns a value that indicates
        /// whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The string representation of a number.</param>
        /// <param name="result">
        /// When this method returns, contains the <see cref="FixedDecimal" /> equivalent to the number that is contained in value,
        /// or 0 if the conversion failed.
        /// </param>
        /// <returns><see cref="true" /> if <see cref="value" /> was converted successfully; otherwise, <see cref="false" />.</returns>
        public static bool TryParse(string value, out FixedDecimal result)
        {
            result = default;
            if (value is null) return false;

            BigInteger scaled;

            var decimalPointIndex = value.IndexOf('.');
            if (decimalPointIndex == -1)
            {
                if (!BigInteger.TryParse(value, out scaled)) return false;
                result = new FixedDecimal(scaled, 0);
                return true;
            }

            if (!BigInteger.TryParse(value.Remove(decimalPointIndex, 1), out scaled)) return false;
            var precision = value.Length - (decimalPointIndex + 1);
            if (precision > byte.MaxValue) return false;
            result = new FixedDecimal(scaled, (byte)precision);
            return true;
        }
    }
}
