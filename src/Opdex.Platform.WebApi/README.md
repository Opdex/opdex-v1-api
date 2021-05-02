# Opdex.Platform.WebApi

Public platform API project and composition root. A standalone API publicly available to access and integrate opdex protocol into external projects.

This composition root references all projects of Opdex.Platform and Opdex.Core namespaces.

## Local Environment Setup

### Create DB

Using [Opdex DB Scripts](), create the Maria DB database and update the connection string in `appsettings.Development.json`.

### CMC API KEY

Generate your own Coin Market Cap API Key and add to `appsettings.Development.json`;

### Cirrus Dev

Clone `StratisFullNode` repository.

Navigate to `Stratis.CirrusMinerD` project and run `dotnet run -devmode=miner` and wait for setup.

Use swagger to find the funded wallet address. Wallet name should be `cirrusdev` and password should be `password`.


### Populate DB Block table with first block

Using Stratis Full Node and Swagger, hit `GetBestBlock` endpoint. Using the return hash value and swagger, use `GetBlock` to get the blocks details of its hash and block height.

Insert that block manually to `Db.block` table.


### Split Coins

Using Stratis Full Node swagger, either send and receive some transactions to yourself to split UXTO's or use the split coins endpoint. 

(This is precautionary so the deployment doesn't have issues midway.)


### Deploy Contracts

Using Opdex swagger, hit `deploy/dev-contracts` and let it run. This process takes about 4-5 minutes while transactions are submitted and waited to be mined.

Once this is done, all necessary contracts are deployed. 


### Process Transactions

Using Opdex swagger, hit `index/process-blocks` to begin syncing from the block manually entered from the previous step for Opdex transactions.

DB must be setup prior with tables and connection strings with the correct configuration.


### Use DB to Populate Postman ENV variables

Look through the database and adjust Postman environment variables to match your local environment for tokens, pools, market contracts etc. 