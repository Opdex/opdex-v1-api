using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.Transactions
{
    /// <summary>
    /// Defines a labelled smart contract method parameter
    /// </summary>
    public class TransactionQuoteRequestParameter
    {
        private string _label;
        private SmartContractMethodParameter _value;

        public TransactionQuoteRequestParameter(string label, bool value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, byte value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, char value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, string value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, uint value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, int value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, ulong value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, long value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, Address value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, byte[] value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, UInt128 value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, UInt256 value)
        {
            Label = label;
            Value = new SmartContractMethodParameter(value);
        }

        public TransactionQuoteRequestParameter(string label, SmartContractMethodParameter value)
        {
            Label = label;
            Value = value;
        }

        public string Label
        {
            get => _label;
            private set => _label = value.HasValue() ? value : throw new ArgumentNullException("Label must not be null or empty.");
        }

        public SmartContractMethodParameter Value
        {
            get => _value;
            private set => _value = value ?? throw new ArgumentNullException("Value must not be null.");
        }
    }
}
