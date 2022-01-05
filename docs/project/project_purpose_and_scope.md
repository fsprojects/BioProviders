# Project Purpose and Scope
This document outlines the purpose and scope of the BioProviders project. The information provided here does not discuss implementation or specification details and is only intended to outline the motivation behind the project.

## Purpose
The goal of this project is to simplify access to bioinformatic data, beginning with that stored in GenBank and RefSeq. To do so, a Type Provider will be developed in the F# programming language to enable more efficient access, exploration, and manipulation of the GenBank and RefSeq datasets.

The BioProviders project intends to provide a pathway for bioinformatics in the information-rich programming space.

## Scope
### Initial Scope
The BioProviders project aims to provide the bioinformatic data stored in GenBank and RefSeq via Type Providers. This will allow the data to be accessed programmatically with type inference and safety. Particularly, the sequence and annotation fields of all species and their assemblies should be accessible in a simple and consistent way.

To facilitate this, the Type Providers will allow access and exploration at all levels of GenBank's and RefSeq's hierarchical structure (see [Understanding GenBank and RefSeq](understanding_genbank_and_refseq.md)). For example, it is intended that a user will be able to explore the taxonomic groups, species, assemblies, and content of the two databases. 

However, it should be noted that the exploration of the structural levels of GenBank and RefSeq is not the predominant focus of the project. Instead, these levels provide a pathway to access the genomic data of species of interest.

### Extensions
No extensions have been made to the project scope.
