using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.TransactionLogs;

public abstract class TransactionLog
{
    protected internal TransactionLog(TransactionLogType logType, Address contract, int sortOrder)
    {
        if (!logType.IsValid())
        {
            throw new ArgumentOutOfRangeException(nameof(logType));
        }

        if (contract == Address.Empty)
        {
            throw new ArgumentNullException(nameof(contract));
        }

        if (sortOrder < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sortOrder));
        }

        LogType = logType;
        Contract = contract;
        SortOrder = sortOrder;
    }

    protected internal TransactionLog(TransactionLogType logType, ulong id, ulong transactionId, Address contract, int sortOrder)
    {
        LogType = logType;
        Id = id;
        TransactionId = transactionId;
        Contract = contract;
        SortOrder = sortOrder;
    }

    public ulong Id { get; }
    public TransactionLogType LogType { get; }
    public ulong TransactionId { get; private set; }
    public Address Contract { get; }
    public int SortOrder { get; }

    protected internal void SetTransactionId(ulong txId)
    {
        if (TransactionId > 0) throw new InvalidOperationException("TransactionId is already set.");
        if (txId == 0) throw new ArgumentOutOfRangeException(nameof(txId), "TransactionId must be greater than zero.");

        TransactionId = txId;
    }

    public abstract string SerializeLogDetails();
}