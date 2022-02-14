# BioProviders
The BioProviders Project simplifies programmatic access to genomic datasets, enabling bioinformatics applications to be developed with greater ease.

This package contains F# Type Providers for exploring remote genomic data sources and accessing their data files. Parsing of the data is performed by [.NET Bio](https://github.com/dotnetbio/bio). 

## Releases
The first release to NuGet is currently in progresss.

## Support
The following databases and their associated datafiles are currently supported:

| Databases | File Types |
| :-------- |:-----------|
| GenBank   | <ul><li>GenBank Flat File</li></ul> |

## Project Goals
The BioProviders Project has been developed with the following goals in mind:

* **Convenience**: It is important that the BioProviders package is highly accessible and easy to use. Developers should be able to incorporate the tools and functionality of this package into their own projects with minimal overhead. 

* **Flexibility**: For the BioProviders package to be useful in external applications, it must be flexible. Developers should be able to easily access the information they desire, whether that be searching for a particular assembly, all assemblies of a certain species, or all species of a genus. This flexibility must not occur at the cost of type-safety.

* **Reliability**: The BioProviders package must be reliable. Developers should always be able to expect the outcome of the tools provided.


## Examples
### GenBankProvider
![GenBankProvider Demo](https://github.com/AlexKenna/BioProviders/blob/main/img/GenBankProvider_Demo.gif?raw=true)

The BioProviders Type Provider for GenBank allows type-safe access and exploration of the server's structure and data files. This provides the ability to search for genomic assemblies based on taxon (e.g., bacteria, invertebrate), species (e.g., Staphylococcus_borealis) and assembly (e.g. GCA_003042555.1_ASM304255v1).

## Additional Information
There is currently no additional information.
