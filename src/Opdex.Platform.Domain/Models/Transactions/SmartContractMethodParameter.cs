using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.Transactions
{
    /// <summary>
    /// Defines a parameter that can be passed to a smart contract method
    /// </summary>
    public class SmartContractMethodParameter : IEquatable<SmartContractMethodParameter>
    {
        private string _value;
        private SmartContractParameterType _parameterType;

        public SmartContractMethodParameter(bool value)
        {
            Value = value.ToString();
            Type = SmartContractParameterType.Boolean;
        }

        public SmartContractMethodParameter(byte value)
        {
            Value = value.ToString();
            Type = SmartContractParameterType.Byte;
        }

        public SmartContractMethodParameter(char value)
        {
            Value = value.ToString();
            Type = SmartContractParameterType.Char;
        }

        public SmartContractMethodParameter(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value), "String value must not be null.");
            Value = value;
            Type = SmartContractParameterType.String;
        }

        public SmartContractMethodParameter(uint value)
        {
            Value = value.ToString();
            Type = SmartContractParameterType.UInt32;
        }

        public SmartContractMethodParameter(int value)
        {
            Value = value.ToString();
            Type = SmartContractParameterType.Int32;
        }

        public SmartContractMethodParameter(ulong value)
        {
            Value = value.ToString();
            Type = SmartContractParameterType.UInt64;
        }

        public SmartContractMethodParameter(long value)
        {
            Value = value.ToString();
            Type = SmartContractParameterType.Int64;
        }

        public SmartContractMethodParameter(Address value)
        {
            if (value == Address.Empty) throw new ArgumentNullException(nameof(value), "Address value must be set.");
            Value = value.ToString();
            Type = SmartContractParameterType.Address;
        }

        public SmartContractMethodParameter(byte[] value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value), "Byte array value must not be null.");
            Value = BitConverter.ToString(value);
            Type = SmartContractParameterType.ByteArray;
        }

        public SmartContractMethodParameter(UInt128 value)
        {
            Value = value.ToString();
            Type = SmartContractParameterType.UInt128;
        }

        public SmartContractMethodParameter(UInt256 value)
        {
            Value = value.ToString();
            Type = SmartContractParameterType.UInt256;
        }

        public SmartContractMethodParameter(string value, SmartContractParameterType type)
        {
            Value = value;
            Type = type;
        }

        public string Value
        {
            get => _value;
            private set => _value = value.HasValue() ? value : throw new ArgumentNullException("Smart contract parameter value must not be null or empty.");
        }

        public SmartContractParameterType Type
        {
            get => _parameterType;
            private set => _parameterType = value != SmartContractParameterType.Unknown && value.IsValid()
                                                ? value : throw new ArgumentOutOfRangeException("Smart contract parameter type must be known and valid.");
        }

        public string Serialize() => $"{(uint)Type}#{Value}";

        public bool Equals(SmartContractMethodParameter other) => _value == other._value && _parameterType == other._parameterType;

        public override bool Equals(object obj) => obj is SmartContractMethodParameter parameter && Equals(parameter);

        public override int GetHashCode() => HashCode.Combine(_value, _parameterType);

        public override string ToString() => Serialize();

        public static SmartContractMethodParameter Deserialize(string value)
        {
            var values = value.Split('#', 2);
            return new SmartContractMethodParameter(values[1], (SmartContractParameterType)int.Parse(values[0]));
        }
    }
}
