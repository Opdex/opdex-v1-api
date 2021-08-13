using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Common.Models
{
    public readonly struct Address : IEquatable<string>, IEquatable<Address>
    {
        private string Value { get; }

        public static Address Zero => new Address(null);

        public Address(string value)
        {
            // If provided, should be > 40 but < 42 characters. 42 for potential ETH addresses if ever necessary.
            if (value.HasValue() && (value.Length < 30 || value.Length > 42))
            {
                throw new ArgumentException("Invalid address.");
            }

            this.Value = value.HasValue() ? value : null;
        }

        public static bool operator ==(Address a, Address b)
        {
            if (a.Value == null)
            {
                return b.Value == null;
            }

            return a.Value.Equals(b.Value);
        }

        public static bool operator !=(Address a, Address b)
        {
            return !(a == b);
        }

        public static implicit operator Address(string value)
        {
            return new Address(value);
        }

        public static explicit operator string(Address value)
        {
            return value.Value;
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return this.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Address other && Equals(other);
        }

        public bool Equals(Address other)
        {
            return Value == other.Value;
        }

        public bool Equals(string other)
        {
            return Value == other;
        }
    }
}
