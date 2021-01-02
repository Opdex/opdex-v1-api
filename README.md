# OpdexApi

Opdex open source platform and indexer repository. 

## Opdex.Platform Namespace

### WebApi Project

Opdex public API for read access to Opdex indexed data. 

- Will have no rate limits for official Opdex referrers
- Will have API keys for 3rd party integrations
- _Might_ need Admin API keys for internal endpoint access (for accessing indexer API, depending on internal admin portal project)

## Opdex.Indexer Namespace

### WebApi Project

Indexes opdex pairs, transactions and transaction events. Has private, internal endpoint access when deployed. 

- Currently meant to run as a single instance non-intensive background task.

- Currently pings Cirrus Full Node (CFN) every 3 seconds for new blocks. Indexes all opdex related transactions and effects.

## Opdex.Core Namespace

Includes library projects for different domain layers referenced in Opdex.Indexer and Opdex.Platform namespace projects.