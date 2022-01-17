namespace Opdex.Platform.WebApi.Models.Responses.Transactions;

public interface ITransactionResponseModel
{
    TransactionErrorResponseModel Error { get; set; }
}
