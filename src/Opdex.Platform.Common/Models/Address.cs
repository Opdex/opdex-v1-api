using Opdex.Platform.Common.Converters;
using Opdex.Platform.Common.Extensions;
using System;
using System.ComponentModel;

namespace Opdex.Platform.Common.Models;

[TypeConverter(typeof(AddressTypeConverter))]
public readonly struct Address : IEquatable<string>, IEquatable<Address>
{
    private const string DevZero = "P8bB9yPr3vVByqfmM5KXftyGckAtAdu6f8";
    private const string TestZero = "t6vc3nrbAurGs3i17HJUavZuw4ioKTiFCE";
    private const string MainZero = "CGTta3M4t3yXu8uRgkKvaWd2d8DQvDPnpL";
    private const string CRS = "CRS";
    private const int MinLength = 30;
    private const int MaxLength = 42;

    private string Value { get; }

    public static readonly Address Cirrus = new(CRS);

    public static readonly Address Empty = new(default);

    public bool IsZero => Value is DevZero or TestZero or MainZero;

    public Address(string value)
    {
        // If provided, should be >= 30 but <= 42 characters. 42 for potential ETH addresses if ever necessary.
        if (value.HasValue() && value != CRS && (value.Length < MinLength || value.Length > MaxLength))
        {
            throw new ArgumentException("Invalid address.");
        }

        Value = value.HasValue() ? value : default;
    }

    public static bool operator ==(Address a, Address b)
    {
        return a.Equals(b);
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
        return Value is null ? "".GetHashCode() : Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value ?? "";
    }

    public override bool Equals(object obj)
    {
        return obj is Address a && Equals(a) || obj is string b && Equals(b);
    }

    public bool Equals(Address other)
    {
        return Value == other.Value;
    }

    public bool Equals(string other)
    {
        return Value == other;
    }

    public static bool TryParse(string value, out Address address)
    {
        address = Empty;
        if (!value.HasValue()) return true;
        if (value == Cirrus.Value)
        {
            address = Cirrus;
            return true;
        }

        if (value.Length < MinLength || value.Length > MaxLength) return false;

        address = new Address(value);

        return true;
    }
}
