using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Common.Models
{
    public readonly struct Address : IEquatable<string>
    {
        private string Value { get; }

        public Address(string value)
        {
            // Cheap check based on value and size, larger size for potential ETH compatibility if necessary
            if (!value.HasValue() || value.Length < 30 || value.Length > 42)
            {
                throw new ArgumentException("Invalid address.");
            }

            this.Value = value;
        }

        public static bool operator ==(Address a, Address b)
        {
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
