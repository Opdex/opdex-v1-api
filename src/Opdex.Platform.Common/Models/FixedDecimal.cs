using System;
using System.Numerics;

namespace Opdex.Platform.Common.Models
{
    /// <summary>Represents an arbitrarily large signed fixed-point number.</summary>
    /// <remarks>The number is stored with a maximum precision of 255.</remarks>
    public readonly struct FixedDecimal : IComparable, IComparable<FixedDecimal>, IEquatable<FixedDecimal>
    {
        public static readonly FixedDecimal Zero = default;

        /// <summary>Creates a fixed-point number using the parts of its raw representation.</summary>
        /// <param name="scaledValue">The value scaled by 10^<see cref="precision" />.</param>
        /// <param name="precision">The amount of decimal places after the decimal point.</param>
        public FixedDecimal(BigInteger scaledValue, byte precision)
        {
            ScaledValue = scaledValue;
            Precision = precision;
        }

        /// <summary>Underlying value which represents the number, scaled by 10^<see cref="Precision" />.</summary>
        public BigInteger ScaledValue { get; }

        /// <summary>The amount of decimal places after the decimal point.</summary>
        public byte Precision { get; }

        /// <inheritdoc />
        /// <exception cref="OutOfMemoryException">Thrown if either underlying value is too large.</exception>
        public int CompareTo(FixedDecimal other)
        {
            var factor = Precision - other.Precision;
            return factor >= 0 ? ScaledValue.CompareTo(Scale(other.ScaledValue, factor))
                               : Scale(ScaledValue, factor * -1).CompareTo(other.ScaledValue);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Thrown if <see cref="obj" /> is a type other than <see cref="FixedDecimal" /> and non-null.</exception>
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
            return factor >= 0 ? ScaledValue == Scale(other.ScaledValue, factor)
                               : Scale(ScaledValue, factor * -1) == other.ScaledValue;
        }

        /// <inheritdoc />
        /// <exception cref="OutOfMemoryException">Thrown if either underlying value is too large.</exception>
        public override bool Equals(object obj) => obj is FixedDecimal other && Equals(other);

        /// <inheritdoc />
        /// <exception cref="OutOfMemoryException">Thrown if the underlying value is too large.</exception>
        public override int GetHashCode() => Scale(ScaledValue, byte.MaxValue - Precision).GetHashCode();

        /// <inheritdoc />
        public override string ToString()
        {
            var stringified = ScaledValue.ToString();
            if (Precision == 0) return stringified;

            var padded = stringified.PadLeft(Precision, '0');
            var decimalIndex = padded.Length - Precision;
            var result = padded.Insert(decimalIndex, decimalIndex != 0 ? "." : "0."); // include leading 0
            return result;
        }

        /// <summary>Resizes the precision of the <see cref="FixedDecimal" /> to another value.</summary>
        /// <param name="precision">The desired number of decimal places.</param>
        /// <returns>A <see cref="FixedDecimal" /> with the desired precision.</returns>
        /// <exception cref="OutOfMemoryException">Thrown if the underlying value is too large.</exception>
        public FixedDecimal Resize(byte precision)
        {
            if (Precision == precision) return this;
            if (Precision > precision) throw new NotImplementedException("Reducing precision requires support for division.");

            var factor = precision - Precision;
            return new FixedDecimal(Scale(ScaledValue, factor), precision);
        }

        /// <exception cref="OutOfMemoryException">Thrown if the underlying value is too large.</exception>
        private static BigInteger Scale(BigInteger value, int factor) => value * BigInteger.Pow(10, factor);

        public static bool operator ==(FixedDecimal a, FixedDecimal b) => a.Equals(b);
        public static bool operator !=(FixedDecimal a, FixedDecimal b) => !a.Equals(b);
        public static bool operator >(FixedDecimal a, FixedDecimal b) => a.CompareTo(b) > 0;
        public static bool operator <(FixedDecimal a, FixedDecimal b) => a.CompareTo(b) < 0;
        public static bool operator >=(FixedDecimal a, FixedDecimal b) => a.CompareTo(b) >= 0;
        public static bool operator <=(FixedDecimal a, FixedDecimal b) => a.CompareTo(b) <= 0;

        public static implicit operator FixedDecimal(int value) => new FixedDecimal(value, 0);
        public static implicit operator FixedDecimal(uint value) => new FixedDecimal(value, 0);
        public static implicit operator FixedDecimal(long value) => new FixedDecimal(value, 0);
        public static implicit operator FixedDecimal(ulong value) => new FixedDecimal(value, 0);
        public static implicit operator FixedDecimal(BigInteger value) => new FixedDecimal(value, 0);

        /// <summary>Converts the string representation of a number to its <see cref="FixedDecimal" /> equivalent.</summary>
        /// <param name="value">The stringified decimal value</param>
        /// <returns>A value that is equivalent to the number specified in the <see cref="value" /> parameter.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        /// <exception cref="FormatException" />
        public static FixedDecimal Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

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
            if (string.IsNullOrWhiteSpace(value)) return false;

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
