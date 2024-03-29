using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models.Transactions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

public class LocalCallResponseDto
{
    public IList<InternalTransfer> InternalTransfers { get; set; } = new List<InternalTransfer>();
    public IList<TransactionLogSummaryDto> Logs { get; set; }
    public GasConsumed GasConsumed { get; set; }
    public bool Revert { get; set; }
    public Error ErrorMessage { get; set; }
    public object Return { get; set; }

    public bool HasError => ErrorMessage?.Value?.HasValue() ?? false;

    // This should be done better in general
    public T DeserializeValue<T>()
    {
        if (HasError) throw new Exception(ErrorMessage.Value);

        if (Return == null) return default;

        var value = JsonConvert.SerializeObject(Return);
        return JsonConvert.DeserializeObject<T>(value);
    }

    public bool TryDeserializeValue<T>(out T value)
    {
        value = default;

        if (Revert) return false;

        // if the call was successful, the only way return value can be null is if it's a reference type
        if (Return is null) return !typeof(T).IsValueType;

        try
        {
            var serializedValue = JsonConvert.SerializeObject(Return, Serialization.DefaultJsonSettings);
            value = JsonConvert.DeserializeObject<T>(serializedValue, Serialization.DefaultJsonSettings);
        }
        catch (Exception)
        {
            return false;
        }

        if (value is Enum enumValue) return enumValue.IsValid();

        return true;
    }
}

public class Error
{
    public string Value { get; set; }
}

public class GasConsumed
{
    public uint Value { get; set; }
}

public class InternalTransfer
{
    public string From { get; set; }
    public string To { get; set; }
    public ulong Value { get; set; }
}
