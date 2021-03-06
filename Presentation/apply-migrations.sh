#!/bin/bash

# https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli#update-the-database

dotnet ef database update -c ConfigurationDbContext && \
dotnet ef database update -c PersistedGrantDbContext && \
dotnet ef database update -c SecureDbContext
