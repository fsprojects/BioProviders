# BioProviders
The BioProviders Project simplifies programmatic access to genomic datasets, enabling bioinformatics applications to be developed with greater ease.

This package contains F# Type Providers for exploring remote genomic data sources and accessing their data files. Parsing of the data is performed by [.NET Bio](https://github.com/dotnetbio/bio). 

## Releases
The first release to NuGet is currently in progress.

## Support
The following databases and their associated datafiles are currently supported:

| Databases | File Types |
| :-------- |:-----------|
| GenBank   | - GenBank Flat File |

## Examples
### GenBankProvider
![GenBankProvider Demo](https://github.com/AlexKenna/BioProviders/blob/main/img/GenBankProvider_Demo.gif?raw=true)

The BioProviders Type Provider for GenBank allows type-safe access and exploration of the server's structure and data files. This provides the ability to search for genomic assemblies based on taxon (e.g., bacteria, invertebrate), species (e.g., Staphylococcus_borealis) and assembly (e.g. GCA_003042555.1_ASM304255v1).

## Additional Information
There is currently no additional information.
