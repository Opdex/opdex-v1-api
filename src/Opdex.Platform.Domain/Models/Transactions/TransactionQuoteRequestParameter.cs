using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Domain.Models.Transactions
{
    public class TransactionQuoteRequestParameter
    {
        public TransactionQuoteRequestParameter(string label, string value, SmartContractParameterType type)
        {
            if (!label.HasValue())
            {
                throw new ArgumentNullException(nameof(label));
            }

            if (!value.HasValue())
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!type.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            Label = label;
            Value = value;
            Type = type;
        }

        public TransactionQuoteRequestParameter(string label, string encodedValue)
        {
            if (!label.HasValue())
            {
                throw new ArgumentNullException(nameof(label));
            }

            (SmartContractParameterType type, string decodedValue) = DeserializeParameterValue(encodedValue);

            Label = label;
            Value = decodedValue;
            Type = type;
        }

        public string Label { get; }
        public string Value { get; }
        public SmartContractParameterType Type { get; }
        public string Serialized => Value.ToSmartContractParameter(Type);

        private (SmartContractParameterType, string) DeserializeParameterValue(string value)
        {
            if (!value.HasValue() || !value.Contains('#'))
            {
                throw new ArgumentException("Invalid parameter value", nameof(value));
            }

            var parts = value.Split("#");

            if (!int.TryParse(parts[0], out var type) || !((SmartContractParameterType)type).IsValid())
            {
                throw new Exception("Unable to parse parameter type.");
            }

            return ((SmartContractParameterType)type, parts[1]);
        }
    }
}
