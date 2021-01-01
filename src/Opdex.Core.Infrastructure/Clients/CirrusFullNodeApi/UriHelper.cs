namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi
{
    public static class UriHelper
    {
        public static class SmartContracts
        {
            public const string GetContractCode = "SmartContracts/code?address={0}";
            public const string GetContractStorageItem = "SmartContracts/storage?ContractAddress={0}&StorageKey={1}&DataType={2}";
            public const string GetContractBalance = "SmartContracts/balance?address={0}";
            public const string GetTransactionReceipt = "SmartContracts/receipt?txHash={0}";
            public const string GetContractReceiptSearch = "SmartContracts/receipt-search?contractAddress={0}&eventName={1}&fromBlock={2}&toBlock={3}";
            public const string LocalCall = "SmartContracts/local-call";
        }

        public static class BlockStore
        {
            public const string GetBlockByHash = "BlockStore/block?hash={0}&OutputJson={1}";
        }
    }
}