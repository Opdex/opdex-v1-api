# Opdex Platform and Indexer API

Two API composition roots for the platform API and indexer API using Domain Driven Design and CQRS design patterns.

## Architecture

This repository manages 3 core namespaces, 2 composition roots (executable projects) and 1 shared namespace between the two. 

- Opdex.Platform - Composition Root for Platform API
- Opdex.Indexer - Composition Root for Indexer API
- Opdex.Core - Shared between Platform and Indexer

### Opdex.Platform Namespace

The platform API is the open gateway to opdex backend infrastructure and services. This API is readonly and can fetch Opdex client UI data from indexed data and other 3rd party integrations such as the Cirrus Full Nodes and Bitcore Nodes.

### Opdex.Indexer Namespace

The Indexer API is hosted privately and only can be internally accessed. This API is responsible for reading and persisting any relevant transactions to Opdex databases for quick reads from the Platform API.

### Opdex.Core Namespace

Includes library projects for different domain layers referenced in Opdex.Indexer and Opdex.Platform namespace projects.
