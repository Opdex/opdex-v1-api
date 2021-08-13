using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Common.Models
{
    public struct Address : IComparable
    {
        private readonly string value;

        public Address(string value)
        {
            // Cheap check based on value and size, larger size for potential ETH compatibility if necessary
            if (!value.HasValue() || value.Length < 30 || value.Length > 42)
            {
                throw new ArgumentException("Invalid address.");
            }

            this.value = value;
        }

        public static bool operator ==(Address a, Address b)
        {
            return a.value.CompareTo(b.value) == 0;
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
            return value.value;
        }

        public int CompareTo(object b)
        {
            return String.Compare(this.value, ((Address)b).value, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public override string ToString()
        {
            return this.value;
        }
    }
}
