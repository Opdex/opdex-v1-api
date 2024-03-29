namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi;

public static class CirrusUriHelper
{
    public static class SmartContracts
    {
        public const string GetContractCode = "SmartContracts/code?address={0}";
        public const string GetContractStorageItem = "SmartContracts/storage?ContractAddress={0}&StorageKey={1}&DataType={2}";
        public const string GetContractBalance = "SmartContracts/balance?address={0}";
        public const string GetTransactionReceipt = "SmartContracts/receipt?txHash={0}";
        public const string GetContractReceiptSearch = "SmartContracts/receipt-search?contractAddress={0}&eventName={1}&fromBlock={2}";
        public const string LocalCall = "SmartContracts/local-call";
    }

    public static class SmartContractWallet
    {
        public const string Call = "SmartContractWallet/call";
        public const string Create = "SmartContractWallet/create";
    }

    public static class BlockStore
    {
        public const string GetBlockByHash = "BlockStore/block?hash={0}&OutputJson={1}&showTransactionDetails={2}";
        public const string GetAddressesBalances = "BlockStore/getaddressesbalances?addresses={0}";
    }

    public static class Consensus
    {
        public const string GetBestBlockHash = "Consensus/getbestblockhash";
        public const string GetBlockHash = "Consensus/getblockhash?height={0}";
    }

    public static class Node
    {
        public const string Status = "Node/status";
        public const string GetRawTransaction = "Node/getrawtransaction?trxid={0}&verbose={1}";
    }

    public static class Wallet
    {
        public const string VerifyMessage = "Wallet/verifymessage";
    }


    public static class SupportedContracts
    {
        public const string List = "SupportedContracts/list?networkType={0}";
    }
}
