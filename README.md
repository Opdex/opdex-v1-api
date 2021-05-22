# Opdex Platform API

Web API project to ingest Opdex smart contract transactions for analytical data, transaction quotes and building unsigned transactions.

## Local Environment Setup


### Auth

Add a bearer token signing key to your configuration, by running `dotnet user-secrets set AuthConfiguration:Opdex:SigningKey ~y0Ur%sEcr3T*k3Y~`. The key can be any string of length 16 or more characters.


### Create DB

Using [Opdex DB Scripts](https://github.com/Opdex/opdex-db-scripts), create the Maria DB database then set the connection string in your configuration using `dotnet user-secrets set OpdexConfiguration:ConnectionString "Server=; Port=; Database=; Uid=; Pwd=;"`, providing the correct credentials.


### CMC API KEY

Generate your own Coin Market Cap API Key then set this in your configuration using `dotnet user-secrets set CoinMarketCapConfiguration:ApiKey [your-API-key]`.


### Cirrus Dev

Clone `StratisFullNode` repository.

Navigate to `Stratis.CirrusMinerD` project and run `dotnet run -devmode=miner` and wait for it to start up.

Use the `/api/SmartContractWallet/account-addresses` swagger endpoint to find the funded wallet address.

Wallet name should be `cirrusdev` and password should be `password`.


### Deploy Contracts

Using Opdex swagger, hit `deploy/dev-contracts` and let it run.

This process takes about 5 minutes while transactions are submitted and mined. After the contracts are deployed, indexing of all transaction details will begin.

Once this process is done you'll have a local environment with deployed contracts and a fully synced database.


### Processing Transactions

The indexing process is trigger based so it must be manually called in a local environment.

Using Opdex swagger, hit `index/process-latest-blocks` to begin syncing from the most recently synced block to the tip of the Cirrus chain.

