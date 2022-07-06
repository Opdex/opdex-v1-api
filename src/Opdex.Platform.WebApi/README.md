# Opdex.Platform.WebApi

Public platform API project and composition root. A standalone API publicly available to access and integrate opdex protocol into external projects.

## Running the API

### Dependencies

#### Required
- .NET 6
- Stratis Cirrus full node
- Opdex Auth API
- Azure Subscription
- Azure SignalR service
- Azure Key Vault service
- MySQL server

#### Optional
- CoinMarketCap API Key

### Getting Started

1. Set up configuration values. These can be configured as either environment variables, user secrets or key vault secret.

| Configuration Key                        | Description                                                                                     | Secure |
|------------------------------------------|-------------------------------------------------------------------------------------------------|--------|
| ApplicationInsights:InstrumentationKey   | Optional application insights instrumentation key, for collecting logs                          | No     |
| FeatureManagement:CoinMarketCapPriceFeed | Flag determining whether to use CoinMarketCap as a primary price reference                      | No     |
| AuthConfiguration:Issuer                 | Hostname of the hosted auth API                                                                 | No     |
| AuthConfiguration:AdminKey               | A random 16-octet secret for executing admin endpoints                                          | Yes    |
| CirrusConfiguration:ApiUrl               | Base URL for the cirrus full node API                                                           | No     |
| CirrusConfiguration:ApiPort              | Port number used to access the cirrus full node API                                             | No     |
| DatabaseConfiguration:ConnectionString   | MySQL platform API database connection string                                                   | Yes    |
| EncryptionConfiguration:Key              | A random 16-octet secret for encrypting data                                                    | Yes    |
| OpdexConfiguration:Network               | The Cirrus full node network type, either: DEVNET, TESTNET, MAINNET                             | No     |
| OpdexConfiguration:ApiUrl                | The base address of the hosted API                                                              | No     |
| OpdexConfiguration:CommitHash            | Commit hash of the currently deployed code, ideally set by CI/CD pipeline                       | No     |
| IndexerConfiguration:Enabled             | Flag that determines whether the indexer is enabled                                             | No     |
| CoinMarketCapConfiguration:ApiKey        | CoinMarketCap API key, required only if FeatureManagement:CoinMarketCapPriceFeed is set to true | No     |
| MaintenanceConfiguration:Locked          | Flag that determines if the API is configured for maintenance                                   | No     |
| Azure:SignalR:ConnectionString           | ConnectionString for Azure SignalR instance                                                     | No     |
| Azure:KeyVault:Name                      | Name of the Azure key vault used for storing secrets                                            | No     |

2. Ensure all dependencies are set up and configured correctly
3. Run the API using `dotnet run`
