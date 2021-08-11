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

        public string Label { get; }
        public string Value { get; }
        public SmartContractParameterType Type { get; }
        public string Serialized => Value.ToSmartContractParameter(Type);
    }
}
